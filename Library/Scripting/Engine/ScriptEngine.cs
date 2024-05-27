using System.Reflection;
using BlocklyNet.Scripting.Parsing;
using BlocklyNet.User;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// The script execution engine. There can be at most one active
/// script at any time.
/// </summary>
/// <param name="context">Execution context of the engine.</param>
/// <param name="_rootProvider">Dependency injection manager.</param>
/// <param name="logger">Logging helper.</param>
/// <param name="parser">Script parser to use.</param>
public partial class ScriptEngine(IServiceProvider _rootProvider, IScriptParser parser, ILogger<ScriptEngine> logger, IScriptEngineNotifySink? context = null) : IScriptEngine, IScriptSite, IDisposable
{
    /// <inheritdoc/>
    public ILogger Logger => logger;

    /// <summary>
    /// Synchronize modifying the result.
    /// </summary>
    private readonly Semaphore _lock = new(1, 1);

    /// <summary>
    /// The active script.
    /// </summary>
    private IScriptInstance? _active;

    /// <summary>
    /// Set when the one and only script is done.
    /// </summary>
    private bool _done = false;

    /// <inheritdoc/>
    public IServiceProvider ServiceProvider => _activeScope?.ServiceProvider ?? _rootProvider;

    private IServiceScope? _activeScope;

    /// <summary>
    /// Manages the cancel process.
    /// </summary>
    private CancellationTokenSource _cancel = new();

    /// <inheritdoc />
    public CancellationToken Cancellation => _cancel.Token;

    /// <inheritdoc />
    public IScript? CurrentScript => _active;

    /// <inheritdoc />
    public IScript? MainScript => _active;

    /// <summary>
    /// Last progress seen.
    /// </summary>
    private ScriptProgress? _lastProgress;

    /// <summary>
    /// Script parser to use.
    /// </summary>
    public IScriptParser Parser { get; } = parser;

    /// <inheritdoc/>
    public string Start(StartScript request, string userToken, StartScriptOptions? options = null)
    {
        Logger.LogTrace("Script '{Name}' should be started for {Token}.", request.Name, userToken);

        try
        {
            /* Try to create the script instance. */
            if (Activator.CreateInstance(request.GetScriptType(), request, this) is not Script script)
                throw new ArgumentException("bad script for '{Name}' request.", request.Name);

            /* Process options. */
            script.ShouldStopNow = options?.ShouldStopNow;

            using (_lock.Wait())
            {
                /* There can be only one active script. */
                if (_active != null)
                    throw new InvalidOperationException("another script is already executing.");

                /* Reset the internal state to the new script. */
                _inputResponse?.SetException(new OperationCanceledException());

                _active = script;
                _allProgress.Clear();
                _done = false;
                _inputRequest = null;
                _inputResponse = null;
                _lastProgress = null;
                _lastProgressValue = null;

                /* Do additional cleanup */
                OnPrepareStart();

                _activeScope = _rootProvider.CreateScope();

                /* If the notion of a user is enabled attach the user to the script execution threads. */
                ServiceProvider.GetService<ICurrentUser>()?.FromToken(userToken);

                Logger.LogTrace("Script '{Name}' started as {JobId}.", request.Name, script.JobId);

                /* Process the script on a separate thread. */
                _cancel = new();

                ThreadPool.QueueUserWorkItem(RunScript);

                /* Inform all. */
                context?.Send(ScriptEngineNotifyMethods.Started, new ScriptInformation
                {
                    JobId = script.JobId,
                    ModelType = request.ModelType,
                    Name = request.Name
                })
                .ContinueWith(t => Logger.LogError("Failed to report active script: {Exception}", t.Exception?.Message), TaskContinuationOptions.NotOnRanToCompletion);

                return script.JobId;
            }
        }
        catch (Exception e)
        {
            Logger.LogError("Unable to start script '{Name}': {Exception}", request.Name, e.Message);

            throw;
        }
    }

    /// <summary>
    /// Allow customization to prepare for a script execution.
    /// </summary>
    protected virtual void OnPrepareStart() { }

    /// <inheritdoc/>
    public void Cancel(string jobId)
    {
        using (_lock.Wait())
        {
            if (_active == null || _active.JobId != jobId)
                throw new ArgumentException("not the sctive script", nameof(jobId));

            /* Report the result. */
            Logger.LogTrace("User cancelled script {JobId}", jobId);

            _cancel.Cancel();

            /* Abort pending input. */
            if (_inputResponse != null && _inputRequest != null)
                SetUserInput(new() { JobId = jobId, Key = _inputRequest.Key }, false);

            /* Do custom cleanup. */
            OnCancel(jobId);
        }
    }

    /// <summary>
    /// Cleanup during a cancel operation.
    /// </summary>
    /// <param name="jobId">The script that has been cancelled.</param>
    protected virtual void OnCancel(string jobId) { }

    /// <summary>
    /// Execute the main script.
    /// </summary>
    /// <param name="state">Not used.</param>
    private void RunScript(object? state)
    {
        /* Script to use. */
        var script = _active;

        if (script == null)
        {
            Logger.LogError("No active script - internal server error, contact your seller.");

            return;
        }

        /* Potentiall error code. */
        Exception? error = null;

        try
        {
            /* Now we can synchronously execute the script. */
            script.Execute().Wait();

            /* Check for cancel. */
            _cancel.Token.ThrowIfCancellationRequested();
        }
        catch (Exception e)
        {
            /* Something went wrong. */
            if (e is AggregateException aggregation && aggregation.InnerExceptions.Count == 1)
                error = aggregation.InnerExceptions.Single();
            else
                error = e;

            if (error is TargetInvocationException target)
                error = target.InnerException ?? target;

            Logger.LogError("Failed to execute script {JobId}: {Exception}", script.JobId, error.Message);
        }
        try
        {
            _done = true;

            /* Forward the information on the now terminated script. */
            var request = script.GetRequest();
            var requestName = request.Name;

            var task = error == null
                ? context?.Send(ScriptEngineNotifyMethods.Done, new ScriptDone
                {
                    JobId = script.JobId,
                    ModelType = request.ModelType,
                    Name = requestName,
                })
                : context?.Send(ScriptEngineNotifyMethods.Error, new ScriptError
                {
                    ErrorMessage = error.Message,
                    JobId = script.JobId,
                    ModelType = request.ModelType,
                    Name = requestName,
                });

            /* In case of any error just log - actually this could be quite a problem. */
            task?.ContinueWith(t => Logger.LogError("Failed to finish script: {Exception}", t.Exception?.Message), TaskContinuationOptions.NotOnRanToCompletion);
        }
        catch (Exception e)
        {
            Logger.LogError("Failed to finish script: {Exception}", e.Message);
        }
        finally
        {
            /* In case of execution error forget the running job - elsewhere the script must be ended using GetResult(). */
            if (error != null)
                FinishScriptAndGetResult(script.JobId);
        }
    }

    /// <inheritdoc/>
    public object? FinishScriptAndGetResult(string jobId)
    {
        using (_lock.Wait())
        {
            /* Can only get the result for the active script. */
            var script = _active;

            if (script == null || script.JobId != jobId)
                throw new ArgumentException("no the active script", nameof(jobId));

            if (!_done)
                throw new ArgumentException("script not yet finished", nameof(jobId));

            /* Reset active script - result can only be requested once. */
            using (_activeScope) _activeScope = null;

            _active = null;

            /* Report the result. */
            Logger.LogTrace("Processing result for script {JobId}", jobId);

            /* Inform all. */
            context?.Send(ScriptEngineNotifyMethods.Finished, new ScriptFinished
            {
                JobId = script.JobId,
                ModelType = script.GetRequest().ModelType,
                Name = script.GetRequest().Name,
            })
            .ContinueWith(t => Logger.LogError("Failed to report active script: {Exception}", t.Exception?.Message), TaskContinuationOptions.NotOnRanToCompletion);

            return script.Result;
        }
    }

    /// <inheritdoc/>
    public Task<object?> Evaluate(string scriptAsXml, Dictionary<string, object?> presets) =>
        Parser.Parse(scriptAsXml).Evaluate(presets, this);

    /// <inheritdoc/>
    public Task<TResult> Run<TResult>(StartScript request, StartScriptOptions? options = null)
        => StartChild<TResult>(request, _active, options, 0);

    /// <summary>
    /// Finish using this instance.
    /// </summary>
    public void Dispose()
    {
        /* Release system resources. */
        _lock.Dispose();
    }

    /// <inheritdoc/>
    public void Reconnect(IScriptEngineNotifySink client)
    {
        using (_lock.Wait())
        {
            /* Nothing to report. */
            if (_active == null) return;

            /* Has been started. */
            client
                .Send(ScriptEngineNotifyMethods.Current, new ScriptInformation { JobId = _active.JobId, ModelType = _active.GetRequest().ModelType, Name = _active.GetRequest().Name })
                .ContinueWith(t => Logger.LogError("Failed to report active script: {Exception}", t.Exception?.Message), TaskContinuationOptions.NotOnRanToCompletion);

            /* Has some last progress. */
            if (_lastProgress != null)
                client
                    .Send(ScriptEngineNotifyMethods.Progress, _lastProgress)
                    .ContinueWith(t => Logger.LogError("Failed to forward progress: {Exception}", t.Exception?.Message), TaskContinuationOptions.NotOnRanToCompletion);

            /* Is waiting for input. */
            if (_inputRequest != null)
                client
                    .Send(ScriptEngineNotifyMethods.InputRequest, _inputRequest)
                    .ContinueWith(t => Logger.LogError("Failed to request user input for script {JobId}: {Exception}", _inputRequest.JobId, t.Exception?.Message), TaskContinuationOptions.NotOnRanToCompletion);

            /* Script is completed. */
            if (_done)
                client
                    .Send(ScriptEngineNotifyMethods.Done, new ScriptDone { Name = _active.GetRequest().Name, JobId = _active.JobId, ModelType = _active.GetRequest().ModelType })
                    .ContinueWith(t => Logger.LogError("Failed to finish script: {Exception}", t.Exception?.Message), TaskContinuationOptions.NotOnRanToCompletion);
        }
    }
}
