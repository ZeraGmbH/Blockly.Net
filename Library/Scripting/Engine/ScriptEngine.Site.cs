using System.Reflection;
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
    private class ScriptSite(ScriptEngine engine, IScript? parent, int depth) : IScriptSite
    {
        /// <summary>
        /// Last progress information of this child script.
        /// </summary>
        public ProgressDetails? LastProgress = null;

        /// <summary>
        /// The script starting this script.
        /// </summary>
        private readonly IScript? _parent = parent;

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
        public Task<object?> Evaluate(string scriptAsXml, Dictionary<string, object?> presets) =>
            _engine.Parser.Parse(scriptAsXml).Evaluate(presets, this);

        /// <inheritdoc/>
        public Task<TResult> Run<TResult>(StartScript request, StartScriptOptions? options = null)
            => _engine.StartChild<TResult>(request, CurrentScript, options, depth);

        /// <inheritdoc/>
        public Task<T?> GetUserInput<T>(string key, string? type = null) => _engine.GetUserInput<T>(key, type);

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
        public Task<object?> WaitForResult()
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

        /// <inheritdoc/>
        public void Start(StartScript request, StartScriptOptions? options = null)
        {
            /* Create the script instance from the configuration model. */
            if (Activator.CreateInstance(request.GetScriptType(), request, this) is not Script script)
                throw new ArgumentException("bad script for '{Name}' request.", request.Name);

            /* Process options. */
            script.ShouldStopNow = options?.ShouldStopNow;

            /* Start the background execution of the script. */
            ThreadPool.QueueUserWorkItem(RunScript, script);
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
                script.Execute().Wait();

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
            }
            finally
            {
                /* Mark as done and wake up pending requests for result. */
                lock (_resultLock)
                {
                    _done = true;

                    Monitor.PulseAll(_resultLock);
                }
            }
        }
    }
}
