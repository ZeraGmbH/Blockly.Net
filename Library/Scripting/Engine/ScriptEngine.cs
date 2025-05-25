using BlocklyNet.Core.Model;
using BlocklyNet.Scripting.Debugger;
using BlocklyNet.Scripting.Logging;
using BlocklyNet.Scripting.Parsing;
using BlocklyNet.User;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// The script execution engine. There can be at most one active
/// script at any time.
/// </summary>
public partial class ScriptEngine<TLogType> : IScriptEngine, IScriptSite<TLogType>, IGroupManagerSite, IDisposable where TLogType : ScriptLoggingResult, new()
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context">Execution context of the engine.</param>
    /// <param name="rootProvider">Dependency injection manager.</param>
    /// <param name="logger">Logging helper.</param>
    /// <param name="parser">Script parser to use.</param>
    /// <param name="groupManager">Helper to manage execution groups.</param>
    public ScriptEngine(
        IServiceProvider rootProvider,
        IScriptParser parser,
        IGroupManager groupManager,
        ILogger<ScriptEngine<TLogType>> logger,
        IScriptEngineNotifySink? context = null
    )
    {
        _context = context;
        _groupManager = groupManager;
        _rootProvider = rootProvider;
        Logger = logger;
        Parser = parser;

        _groupManager.AttachSite(this);
    }

    private readonly IScriptEngineNotifySink? _context;

    private readonly IServiceProvider _rootProvider;

    private readonly IGroupManager _groupManager;

    /// <inheritdoc/>
    public IScriptEngine Engine => this;

    /// <inheritdoc/>
    public ILogger Logger { get; private set; }

    /// <summary>
    /// Synchronize modifying the result.
    /// </summary>
    protected readonly Semaphore Lock = new(1, 1);

    /// <summary>
    /// The active script.
    /// </summary>
    private IScriptInstance<TLogType>? _active;

    /// <summary>
    /// Set when the one and only script is done.
    /// </summary>
    protected bool Done { get; private set; } = false;


    /// <summary>
    /// Debugger attached to this script.
    /// </summary>
    private IScriptDebugger? _debugger;

    /// <summary>
    /// Exception observed during execution - only valid when _done is set.
    /// </summary>
    private Exception? _error = null;

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
    IScript? IScriptSite.CurrentScript => CurrentScript;

    /// <inheritdoc />
    IScript? IScriptSite.MainScript => MainScript;

    /// <inheritdoc />
    public IScript<TLogType>? CurrentScript => _active;

    /// <inheritdoc />
    public IScript<TLogType>? MainScript => _active;

    /// <summary>
    /// Last progress seen.
    /// </summary>
    private ScriptProgress? _lastProgress;

    /// <summary>
    /// Script parser to use.
    /// </summary>
    public IScriptParser Parser { get; private set; }

    private string _codeHash = null!;

    /// <summary>
    /// Get the current status of the group execution information.
    /// </summary>
    /// <returns>Group execution status.</returns>
    protected ScriptGroupStatus SerializeGroupStatus(bool includeRepeat = false)
        => new()
        {
            CodeHash = _codeHash,
            GroupStatus = _groupManager.Serialize(includeRepeat),
        };

    private void ResetScript(IEnumerable<GroupRepeat>? repeat)
    {
        /* Finish outstanding input requests. */
        _inputResponse?.SetException(new OperationCanceledException());

        /* Prepare for next script to be executed - full reset internal state. */
        Done = false;

        _allProgress.Clear();
        _cancel = new();
        _codeHash = null!;
        _error = null;
        _groupManager.Reset(repeat);
        _inputRequest = null;
        _inputResponse = null;
        _lastProgress = null;
        _progress.Reset();
    }

    /// <inheritdoc/>
    public virtual async Task<string> StartAsync(StartScript request, string userToken, StartScriptOptions? options = null)
    {
        Logger.LogTrace("Script '{Name}' should be started for {Token}.", request.Name, userToken);

        try
        {
            /* Try to create the script instance. */
            if (Activator.CreateInstance(request.GetScriptType(), request, this, options) is not IScriptInstance<TLogType> script)
                throw new ArgumentException("bad script for '{Name}' request.", request.Name);

            using (Lock.Wait())
            {
                /* There can be only one active script. */
                if (_active != null)
                    throw new InvalidOperationException("another script is already executing.");

                /* Reset the internal state to the new script. */
                _active = script;

                ResetScript(options?.GroupResults?.GroupStatus);

                _activeScope = _rootProvider.CreateScope();

                /* If the notion of a user is enabled attach the user to the script execution threads. */
                ServiceProvider.GetService<ICurrentUser>()?.FromToken(userToken);

                /* Do additional cleanup */
                await OnPrepareStartAsync(false);

                Logger.LogTrace("Script '{Name}' started as {JobId}.", request.Name, script.JobId);

                /* Process the script on a separate thread. */
                Task.Factory.StartNew(RunScriptAsync, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Current).Touch();

                /* Inform all. */
                _context?
                    .SendAsync(ScriptEngineNotifyMethods.Started, CreateStartNotification(script))
                    .ContinueWith(
                        t => Logger.LogError("Failed to report active script: {Exception}", t.Exception?.Message),
                        CancellationToken.None,
                        TaskContinuationOptions.NotOnRanToCompletion,
                        TaskScheduler.Current)
                    .Touch();

                return script.JobId;
            }
        }
        catch (Exception e)
        {
            Logger.LogError("Unable to start script '{Name}': {Exception}", request.Name, e.Message);

            throw;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<string> RestartAsync(string jobId, IEnumerable<GroupRepeat>? repeat)
    {
        Logger.LogTrace("Script should be restarted");

        try
        {
            using (Lock.Wait())
            {
                /* There must be some active script. */
                var script = (Script?)_active ?? throw new InvalidOperationException("no script to restart.");

                if (script.JobId != jobId) throw new ArgumentException("not the active script");

                if (!Done) throw new InvalidOperationException("script must be finished to be restarte");

                /* Generate a new job identifier and reset internal state. */
                await script.ResetAsync();

                ResetScript(repeat);

                /* Do additional cleanup */
                await OnPrepareStartAsync(true);

                Logger.LogTrace("Script restarted.");

                /* Process the script on a separate thread. */
                Task.Factory.StartNew(RunScriptAsync, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Current).Touch();

                /* Inform all. */
                _context?
                    .SendAsync(ScriptEngineNotifyMethods.Started, CreateStartNotification(script))
                    .ContinueWith(
                        t => Logger.LogError("Failed to report active script: {Exception}", t.Exception?.Message),
                        CancellationToken.None,
                        TaskContinuationOptions.NotOnRanToCompletion,
                        TaskScheduler.Current)
                    .Touch();

                return script.JobId;
            }
        }
        catch (Exception e)
        {
            Logger.LogError("Unable to restart script: {Exception}", e.Message);

            throw;
        }
    }

    /// <summary>
    /// Allow customization to prepare for a script execution.
    /// </summary>
    /// <param name="restart">Set if the current script is restarted.</param>
    protected virtual Task OnPrepareStartAsync(bool restart) => Task.CompletedTask;

    /// <inheritdoc/>
    public void Cancel(string jobId)
    {
        using (Lock.Wait())
        {
            /* Silent leave if script is already cancelled. */
            if (_active == null) return;

            /* Can only cancel the active script. */
            if (_active.JobId != jobId) throw new ArgumentException("not the active script", nameof(jobId));

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
    private async Task RunScriptAsync()
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
            await script.ExecuteAsync();

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
            /* Make sure child processes will terminate as soon as possible. */
            await _cancel.CancelAsync();

            Done = true;
            _error = error;

            /* Inform debugger. */
            _debugger?.ScriptFinished(_error);

            /* Customize. */
            await OnScriptDoneAsync(script, null);

            /* Forward the information on the now terminated script. */
            var task = _error == null
                ? _context?.SendAsync(ScriptEngineNotifyMethods.Done, CreateDoneNotification(script))
                : _context?.SendAsync(ScriptEngineNotifyMethods.Error, CreateErrorNotification(script, _error));

            /* In case of any error just log - actually this could be quite a problem. */
            task?.ContinueWith(
                    t => Logger.LogError("Failed to finish script: {Exception}", t.Exception?.Message),
                    CancellationToken.None,
                    TaskContinuationOptions.NotOnRanToCompletion,
                    TaskScheduler.Current)
                .Touch();
        }
        catch (Exception e)
        {
            Logger.LogError("Failed to finish script: {Exception}", e.Message);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual async Task OnScriptDoneAsync(IScriptInstance<TLogType> script, IScript<TLogType>? parent)
    {
        /* Create a log entry. */
        await UpdateResultLogEntryAsync(script, parent, true);

        /* Always propagate error. */
        if (parent != null && parent.ResultForLogging.Result == ScriptExecutionResultTypes.Active && script.ResultForLogging.Result != ScriptExecutionResultTypes.Success)
            await parent.SetResultAsync(ScriptExecutionResultTypes.Failure);
    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual Task OnEngineIdleAsync(IScriptInstance script) => Task.CompletedTask;

    /// <inheritdoc/>
    public async Task<object?> FinishScriptAndGetResultAsync(string jobId, bool keepActive = false)
    {
        using (Lock.Wait())
        {
            /* Can only get the result for the active script. */
            var script = _active;

            if (script == null || script.JobId != jobId)
                throw new ArgumentException("not the active script", nameof(jobId));

            if (!Done)
                throw new ArgumentException("script not yet finished", nameof(jobId));

            /* Only report result - do not finish script. */
            if (!keepActive)
            {
                /* Will now deactivate the script. */
                await OnEngineIdleAsync(script);

                /* Reset active script - result can only be requested once. */
                using (_activeScope) _activeScope = null;

                _active = null;

                /* Report the result. */
                Logger.LogTrace("Finish script {JobId}", jobId);

                /* Inform all. */
                _context?
                    .SendAsync(ScriptEngineNotifyMethods.Finished, CreateFinishNotification(script))
                    .ContinueWith(
                        t => Logger.LogError("Failed to report script result: {Exception}", t.Exception?.Message),
                        CancellationToken.None,
                        TaskContinuationOptions.NotOnRanToCompletion,
                        TaskScheduler.Current)
                    .Touch();
            }

            /* In either case report the latest result from the script. */
            return script.Result;
        }
    }

    /// <inheritdoc/>
    public Task<object?> EvaluateAsync(string scriptAsXml, Dictionary<string, object?> presets)
    {
        _codeHash = BitConverter.ToString(SHA256.HashData(Encoding.UTF8.GetBytes(scriptAsXml)));

        return Parser.Parse(scriptAsXml).EvaluateAsync(presets, this);
    }

    /// <inheritdoc/>
    public Task<TResult> RunAsync<TResult>(StartScript request, StartScriptOptions? options = null)
        => StartChildAsync<TResult>(request, _active, options, 0, _groupManager);

    /// <summary>
    /// Finish using this instance.
    /// </summary>
    public virtual void Dispose()
    {
        /* Release system resources. */
        Lock.Dispose();
    }

    /// <inheritdoc/>
    public void Reconnect(IScriptEngineNotifySink client)
    {
        using (Lock.Wait())
        {
            /* Nothing to report. */
            if (_active == null) return;

            /* Has been started. */
            client
                .SendAsync(ScriptEngineNotifyMethods.Current, CreateCurrentNotification(_active))
                .ContinueWith(
                    t => Logger.LogError("Failed to report active script: {Exception}", t.Exception?.Message),
                    CancellationToken.None,
                    TaskContinuationOptions.NotOnRanToCompletion,
                    TaskScheduler.Current)
                .Touch();

            /* Has some last progress. */
            if (_lastProgress != null)
                client
                    .SendAsync(ScriptEngineNotifyMethods.Progress, _lastProgress)
                    .ContinueWith(
                        t => Logger.LogError("Failed to forward progress: {Exception}", t.Exception?.Message),
                        CancellationToken.None,
                        TaskContinuationOptions.NotOnRanToCompletion,
                        TaskScheduler.Current)
                    .Touch();

            /* Is waiting for input. */
            if (_inputRequest != null)
                client
                    .SendAsync(ScriptEngineNotifyMethods.InputRequest, _inputRequest)
                    .ContinueWith(
                        t => Logger.LogError("Failed to request user input for script {JobId}: {Exception}", _inputRequest.JobId, t.Exception?.Message),
                        CancellationToken.None,
                        TaskContinuationOptions.NotOnRanToCompletion,
                        TaskScheduler.Current)
                    .Touch();

            /* Script is completed - with or without error. */
            if (Done)
                if (_error == null)
                    client
                        .SendAsync(ScriptEngineNotifyMethods.Done, CreateDoneNotification(_active))
                        .ContinueWith(
                            t => Logger.LogError("Failed to report script done: {Exception}", t.Exception?.Message),
                            CancellationToken.None,
                            TaskContinuationOptions.NotOnRanToCompletion,
                            TaskScheduler.Current)
                        .Touch();
                else
                    client
                        .SendAsync(ScriptEngineNotifyMethods.Error, CreateErrorNotification(_active, _error))
                        .ContinueWith(
                            t => Logger.LogError("Failed to report script faulted: {Exception}", t.Exception?.Message),
                            CancellationToken.None,
                            TaskContinuationOptions.NotOnRanToCompletion,
                            TaskScheduler.Current)
                        .Touch();
        }
    }

    /// <inheritdoc/>
    public Task SingleStepAsync(Block block, Context context, ScriptDebuggerStopReason reason)
        => _debugger?.InterceptAsync(block, context, reason) ?? Task.CompletedTask;

    /// <inheritdoc/>
    public void SetDebugger(IScriptDebugger? debugger) => _debugger = debugger;

    /// <inheritdoc/>
    public Task<GroupStatus?> BeginGroupAsync(string key, string? name, string? details) => _groupManager.StartAsync(key, name, details);

    /// <inheritdoc/>
    public Task<GroupStatus> EndGroupAsync(GroupResult result) => _groupManager.FinishAsync(result);

    /// <inheritdoc/>
    public GroupStatus GetGroupStatus(int index) => _groupManager[index];

    /// <inheritdoc/>
    public List<object?>? CreateFlatResultFromGroups() => _groupManager.CreateFlatResults();

    /// <inheritdoc/>
    public Task<List<GroupInfo>> GetGroupsForScriptAsync(string code) => Parser.Parse(code).GetGroupTreeAsync();

    /// <inheritdoc/>
    public virtual Task BeginExecuteGroupAsync(GroupStatus status, bool recover) => Task.CompletedTask;

    /// <inheritdoc/>
    public virtual Task DoneExecuteGroupAsync(GroupStatus status) => Task.CompletedTask;
}
