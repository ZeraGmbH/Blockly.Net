using System.Reflection;
using BlocklyNet.Core.Model;
using BlocklyNet.Scripting.Debugger;
using BlocklyNet.Scripting.Generic;
using Microsoft.Extensions.Logging;

namespace BlocklyNet.Scripting.Engine;

partial class ScriptEngine<TLogType>
{
    /// <summary>
    /// Helper class to manage nested script calls.
    /// </summary>
    protected class ScriptSite : IScriptSite<TLogType>, IGroupManagerSite
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="engine">The main script engine.</param>
        /// <param name="parent">Parent script.</param>
        /// <param name="depth">Nestring depth of the script, at least 1.</param>
        /// <param name="groupManager">Group management for this nested script only.</param>
        public ScriptSite(ScriptEngine<TLogType> engine, IScript<TLogType>? parent, int depth, ISiteGroupManager groupManager)
        {
            _depth = depth;
            _engine = engine;
            _groupManager = groupManager;
            Parent = parent;

            _groupManager.AttachSite(this);
        }

        private readonly ISiteGroupManager _groupManager;

        private readonly int _depth;

        /// <inheritdoc/>
        public IScriptEngine Engine => _engine;

        /// <summary>
        /// Last progress information of this child script.
        /// </summary>
        public ProgressDetails? LastProgress => _progress.Latest;

        /// <summary>
        /// Progress management.
        /// </summary>
        private readonly ProgressManager _progress = new();

        /// <summary>
        /// The script starting this script.
        /// </summary>
        protected readonly IScript<TLogType>? Parent;

        /// <summary>
        /// Synchronize access to the result.
        /// </summary>
        private readonly object _resultLock = new();

        /// <summary>
        /// Set as soon as the script is finished.
        /// </summary>
        private bool _done = false;

        /// <summary>
        /// Set if the script execution failed.
        /// </summary>
        private Exception? _error;

        /// <summary>
        /// The result of the script execution - this will be the value
        /// of the result variable.
        /// </summary>
        private object? _result;

        /// <summary>
        /// Debugger attached to this script.
        /// </summary>
        private IScriptDebugger? _debugger;

        /// <summary>
        /// The main script engine.
        /// </summary>
        private readonly ScriptEngine<TLogType> _engine;

        /// <inheritdoc/>
        public IServiceProvider ServiceProvider => _engine.ServiceProvider;

        /// <inheritdoc/>
        public ILogger Logger => _engine.Logger;

        /// <inheritdoc/>
        public CancellationToken Cancellation => _engine.Cancellation;

        /// <inheritdoc/>
        public IScript<TLogType>? CurrentScript { get; private set; }

        /// <inheritdoc/>
        public IScript<TLogType>? MainScript => _engine.MainScript;

        /// <inheritdoc/>
        IScript? IScriptSite.CurrentScript => CurrentScript;

        /// <inheritdoc/>
        IScript? IScriptSite.MainScript => MainScript;

        /// <inheritdoc/>
        public Task<object?> EvaluateAsync(string scriptAsXml, Dictionary<string, object?> presets)
            => _engine.Parser.Parse(scriptAsXml).EvaluateAsync(presets, this);

        /// <inheritdoc/>
        public Task<GroupStatus?> BeginGroupAsync(string key, string? name, string? details) => _groupManager.StartAsync(key, name, details);

        /// <inheritdoc/>
        public Task<GroupStatus> EndGroupAsync(GroupResult result) => _groupManager.FinishAsync(result);

        /// <inheritdoc/>
        public GroupStatus GetGroupStatus(int index) => _groupManager[index];

        /// <inheritdoc/>
        public Task<TResult> RunAsync<TResult>(StartScript request, StartScriptOptions? options = null)
            => _engine.StartChildAsync<TResult>(request, CurrentScript, options, _depth, _groupManager);

        /// <inheritdoc/>
        public Task<T?> GetUserInputAsync<T>(string key, string? type = null, double? delay = null, bool? required = null)
            => _engine.GetUserInputAsync<T>(key, type, delay, required);

        /// <inheritdoc/>
        public void ReportProgress(object info, double? progress, string? name, bool? addEstimation)
        {
            /* Remember and propagate. */
            _progress.Update(info, progress, name, addEstimation);

            _engine.ReportProgress(info, _depth);
        }

        /// <summary>
        /// Wait for the script to finish and report either the result 
        /// or the exception observed.
        /// </summary>
        /// <returns>Result of the script.</returns>
        public Task<object?> WaitForResultAsync()
        {
            return Task.Run(() =>
            {
                /* Wait for script to finish. */
                while (!_done)
                    lock (_resultLock)
                        Monitor.Wait(_resultLock);

                /* Report execution error. */
                if (_error != null) throw _error;

                /* Report result. */
                return _result;
            });
        }

        /// <summary>
        /// Start a nested script.
        /// </summary>
        /// <param name="request">Script startup information.</param>
        /// <param name="options">Optional options for the script.</param>
        public void Start(StartScript request, StartScriptOptions? options = null)
        {
            Logger.LogTrace("Nested script '{Name}' should be started.", request.Name);

            try
            {
                /* Create the script instance from the configuration model. */
                if (Activator.CreateInstance(request.GetScriptType(), request, this, options) is not IScriptInstance<TLogType> script)
                    throw new ArgumentException("bad script for '{Name}' request.", request.Name);

                /* Start the background execution of the script. */
                Task.Factory.StartNew(() => RunScriptAsync(script), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Current).Touch();
            }
            catch (Exception e)
            {
                Logger.LogError("Unable to start nested script '{Name}': {Exception}", request.Name, e.Message);

                throw;
            }
        }

        /// <summary>
        /// Process the script.
        /// </summary>
        private async Task RunScriptAsync(IScriptInstance<TLogType> script)
        {
            try
            {
                CurrentScript = script;

                /* Run the script and remember the result. */
                await script.ExecuteAsync();

                _result = script.Result;
            }
            catch (Exception e)
            {
                /* Remember the error. */
                if (e is AggregateException aggregation && aggregation.InnerExceptions.Count == 1)
                    _error = aggregation.InnerExceptions.Single();
                else
                    _error = e;

                if (_error is TargetInvocationException target)
                    _error = target.InnerException ?? target;

                Logger.LogError("Failed to execute nested script {JobId}: {Exception}", script.JobId, _error.Message);
            }
            finally
            {
                /* Inform debugger if still active. */
                _debugger?.ScriptFinished(_error);

                /* Customize. */
                await _engine.OnScriptDoneAsync(script, Parent);

                /* Mark as done and wake up pending requests for result. */
                lock (_resultLock)
                {
                    _done = true;

                    Monitor.PulseAll(_resultLock);
                }
            }
        }

        /// <inheritdoc/>
        public Task SingleStepAsync(Block block, Context context, ScriptDebuggerStopReason reason)
            => _debugger?.InterceptAsync(block, context, reason) ?? Task.CompletedTask;

        /// <inheritdoc/>
        public virtual Task BeginExecuteGroupAsync(GroupStatus status, bool recover) => Task.CompletedTask;

        /// <inheritdoc/>
        public virtual Task DoneExecuteGroupAsync(GroupStatus status) => Task.CompletedTask;

        /// <inheritdoc/>
        public Task UpdateLogAsync() => CurrentScript == null ? Task.CompletedTask : _engine.UpdateResultLogEntryAsync(CurrentScript, Parent, false);

        /// <inheritdoc/>
        public void SetDebugger(IScriptDebugger? debugger) => _debugger = debugger;
    }

    /// <summary>
    /// Create a new script site to allow proper customization of the engine.
    /// </summary>
    /// <param name="parent">Parent script.</param>
    /// <param name="depth">Nesting depth.</param>
    /// <param name="groupManager">Execution group management helper.</param>
    /// <returns>The new site.</returns>
    protected virtual ScriptSite CreateSite(IScript<TLogType>? parent, int depth, IGroupManager groupManager)
        => new(this, parent, depth, groupManager);

    /// <summary>
    /// Start a child script.
    /// </summary>
    /// <typeparam name="TResult">Type of the result data.</typeparam>
    /// <param name="request">Script configuration.</param>
    /// <param name="parent">Parent script.</param>
    /// <param name="options">Detailed configuration of the new script.</param>
    /// <param name="depth">Nestring depth of the child.</param>
    /// <param name="groupManager">Parent group manager to allow for any-depth nesting.</param>
    /// <returns>Task on the result.</returns>
    protected virtual async Task<TResult> StartChildAsync<TResult>(StartScript request, IScript<TLogType>? parent, StartScriptOptions? options, int depth, ISiteGroupManager groupManager)
    {
        /* Create execution context. */
        var site = CreateSite(parent, depth + 1, await groupManager.CreateNestedAsync((request as IStartGenericScript)?.ScriptId ?? string.Empty, request.Name));

        using (Lock.Wait())
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
            return (TResult)(await site.WaitForResultAsync())!;
        }
        finally
        {
            using (Lock.Wait())
                if (depth < _allProgress.Count)
                    _allProgress[depth].Remove(site);
        }
    }
}
