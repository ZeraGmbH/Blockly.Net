using System.Reflection;
using BlocklyNet.Scripting.Parsing;
using Microsoft.Extensions.DependencyInjection;

namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// The script execution engine. There can be at most one active
/// script at any time.
/// </summary>
/// <param name="context">Execution context of the engine.</param>
/// <param name="_rootProvider">Dependency injection manager.</param>
/// <param name="parser">Script parser to use.</param>
public partial class ScriptEngine(IServiceProvider _rootProvider, IScriptParser parser, IScriptEngineContext? context = null) : IScriptEngine, IScriptSite, IDisposable
{
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

            _activeScope = _rootProvider.CreateScope();

            /* Process the script on a separate thread. */
            _cancel = new();

            ThreadPool.QueueUserWorkItem(RunScript);

            /* Inform all. */
            context?.Broadcast("ScriptStarted", new ScriptInformation
            {
                JobId = script.JobId,
                ModelType = request.ModelType,
                Name = request.Name
            })
            .ContinueWith(t => { }, TaskContinuationOptions.NotOnRanToCompletion);

            return script.JobId;
        }
    }

    /// <inheritdoc/>
    public void Cancel(string jobId)
    {
        using (_lock.Wait())
        {
            if (_active == null || _active.JobId != jobId)
                throw new ArgumentException("not the sctive script", nameof(jobId));

            _cancel.Cancel();

            /* Abort pending input. */
            if (_inputResponse != null && _inputRequest != null)
                SetUserInput(new() { JobId = jobId, Key = _inputRequest.Key }, false);
        }
    }

    /// <summary>
    /// Execute the main script.
    /// </summary>
    /// <param name="state">Not used.</param>
    private void RunScript(object? state)
    {
        /* Script to use. */
        var script = _active;

        if (script == null) return;

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
        }
        try
        {
            _done = true;

            /* Forward the information on the now terminated script. */
            var request = script.GetRequest();
            var requestName = request.Name;

            var task = error == null
                ? context?.Broadcast("ScriptDone", new ScriptDone
                {
                    JobId = script.JobId,
                    ModelType = request.ModelType,
                    Name = requestName,
                })
                : context?.Broadcast("ScriptError", new ScriptError
                {
                    ErrorMessage = error.Message,
                    JobId = script.JobId,
                    ModelType = request.ModelType,
                    Name = requestName,
                });

            /* In case of any error just log - actually this could be quite a problem. */
            task?.ContinueWith(t => { }, TaskContinuationOptions.NotOnRanToCompletion);
        }
        catch (Exception)
        {
            //
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

            /* Inform all. */
            context?.Broadcast("ScriptFinished", new ScriptFinished
            {
                JobId = script.JobId,
                ModelType = script.GetRequest().ModelType,
                Name = script.GetRequest().Name,
            })
            .ContinueWith(t => { }, TaskContinuationOptions.NotOnRanToCompletion);

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
    /// Start a child script.
    /// </summary>
    /// <typeparam name="TResult">Type of the result data.</typeparam>
    /// <param name="request">Script configuration.</param>
    /// <param name="parent">Parent script.</param>
    /// <param name="options">Detailed configuration of the new script.</param>
    /// <param name="depth">Nestring depth of the child.</param>
    /// <returns>Task on the result.</returns>
    private async Task<TResult> StartChild<TResult>(StartScript request, IScript? parent, StartScriptOptions? options, int depth)
    {
        /* Create execution context. */
        var site = new ScriptSite(this, parent, depth + 1);

        using (_lock.Wait())
        {
            /* Create a new progress entry for this child. */
            while (depth >= _allProgress.Count) _allProgress.Add([]);

            _allProgress[depth].Add(site);
        }

        try
        {
            /* Start the script. */
            site.Start(request, options);

            /* Execute the script and report the result - or exception. */
            return (TResult)(await site.WaitForResult())!;
        }
        finally
        {
            using (_lock.Wait())
                if (depth < _allProgress.Count)
                    _allProgress[depth].Remove(site);
        }
    }

    /// <summary>
    /// Finish using this instance.
    /// </summary>
    public void Dispose()
    {
        /* Release system resources. */
        _lock.Dispose();
    }
}
