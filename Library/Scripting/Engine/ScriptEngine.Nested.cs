using System.Reflection;
using BlocklyNet.Core.Model;
using Microsoft.Extensions.Logging;

namespace BlocklyNet.Scripting.Engine;

public partial class ScriptEngine
{
    /// <summary>
    /// Helper class to manage nested script calls.
    /// </summary>
    /// <param name="engine">The main script engine.</param>
    /// <param name="parent">Parent script.</param>
    /// <param name="depth">Nestring depth of the script, at least 1.</param>
    /// <param name="_groupManager">Group management for this nested script only.</param>
    protected class ScriptSite(ScriptEngine engine, IScript? parent, int depth, IGroupManager _groupManager) : IScriptSite
    {
        /// <inheritdoc/>
        public IScriptEngine Engine => _engine;

        /// <summary>
        /// Last progress information of this child script.
        /// </summary>
        public ProgressDetails? LastProgress = null;

        /// <summary>
        /// The script starting this script.
        /// </summary>
        protected readonly IScript? Parent = parent;

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
        /// The main script engine.
        /// </summary>
        private readonly ScriptEngine _engine = engine;

        /// <inheritdoc/>
        public IServiceProvider ServiceProvider => _engine.ServiceProvider;

        /// <inheritdoc/>
        public ILogger Logger => _engine.Logger;

        /// <inheritdoc/>
        public CancellationToken Cancellation => _engine.Cancellation;

        /// <inheritdoc/>
        public IScript? CurrentScript { get; private set; }

        /// <inheritdoc/>
        public IScript? MainScript => _engine.MainScript;

        /// <inheritdoc/>
        public Task<object?> EvaluateAsync(string scriptAsXml, Dictionary<string, object?> presets) =>
            _engine.Parser.Parse(scriptAsXml).EvaluateAsync(presets, this);

        /// <inheritdoc/>
        public void BeginGroup(string key) => _groupManager.Start(key);

        /// <inheritdoc/>
        public void EndGroup(object? result) => _groupManager.Finish(result);

        /// <inheritdoc/>
        public Task<TResult> RunAsync<TResult>(StartScript request, StartScriptOptions? options = null)
            => _engine.StartChildAsync<TResult>(request, CurrentScript, options, depth);

        /// <inheritdoc/>
        public Task<T?> GetUserInputAsync<T>(string key, string? type = null, double? delay = null)
            => _engine.GetUserInputAsync<T>(key, type, delay);

        /// <inheritdoc/>
        public void ReportProgress(object info, double? progress, string? name)
        {
            /* Remember and propagate. */
            LastProgress = new() { Progress = progress ?? 0, Name = name };

            _engine.ReportProgress(info, depth);
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
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="options"></param>
        public void Start(StartScript request, StartScriptOptions? options = null)
        {
            Logger.LogTrace("Nested script '{Name}' should be started.", request.Name);

            try
            {
                /* Create the script instance from the configuration model. */
                if (Activator.CreateInstance(request.GetScriptType(), request, this, options) is not Script script)
                    throw new ArgumentException("bad script for '{Name}' request.", request.Name);

                /* Start the background execution of the script. */
                ThreadPool.QueueUserWorkItem(RunScript, script);
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
        /// <param name="state">The script in this site.</param>
        private void RunScript(object? state)
        {
            var script = (Script)state!;

            try
            {
                CurrentScript = script;

                /* Run the script and remember the result. */
#pragma warning disable VSTHRD002 // Avoid problematic synchronous waits
                script.ExecuteAsync().Wait();
#pragma warning restore VSTHRD002 // Avoid problematic synchronous waits

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
                /* Customize. */
                _engine.OnScriptDone(script, Parent);

                /* Mark as done and wake up pending requests for result. */
                lock (_resultLock)
                {
                    _done = true;

                    Monitor.PulseAll(_resultLock);
                }
            }
        }

        /// <inheritdoc/>
        public Task SingleStepAsync(Block block) => Task.CompletedTask;
    }

    /// <summary>
    /// Create a new script site to allow proper customization of the engine.
    /// </summary>
    /// <param name="parent">Parent script.</param>
    /// <param name="depth">Nesting depth.</param>
    /// <returns>The new site.</returns>
    protected virtual ScriptSite CreateSite(IScript? parent, int depth) => new(this, parent, depth, _groupManager.CreateNested());

    /// <summary>
    /// Start a child script.
    /// </summary>
    /// <typeparam name="TResult">Type of the result data.</typeparam>
    /// <param name="request">Script configuration.</param>
    /// <param name="parent">Parent script.</param>
    /// <param name="options">Detailed configuration of the new script.</param>
    /// <param name="depth">Nestring depth of the child.</param>
    /// <returns>Task on the result.</returns>
    protected virtual async Task<TResult> StartChildAsync<TResult>(StartScript request, IScript? parent, StartScriptOptions? options, int depth)
    {
        /* Create execution context. */
        var site = CreateSite(parent, depth + 1);

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
