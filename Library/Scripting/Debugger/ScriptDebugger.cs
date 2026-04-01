using BlocklyNet.Core.Model;
using BlocklyNet.Scripting.Engine;
using BlocklyNet.Scripting.Generic;
using Microsoft.Extensions.Logging;

namespace BlocklyNet.Scripting.Debugger;

/// <summary>
/// Helper class to implement debuggers.
/// </summary>
public abstract class ScriptDebugger : IScriptDebugger, IDisposable
{
    /// <summary>
    /// Logging helper.
    /// </summary>
    protected readonly ILogger<ScriptDebugger> Logger;

    internal ILogger<ScriptDebugger> InternalLogger => Logger;

    /// <summary>
    /// Synchronizer to allow for parallel script execution.
    /// </summary>
    private readonly Semaphore _sync = new(1, 1);

    /// <summary>
    /// All active breakpoints.
    /// </summary>
    public IScriptBreakpoints Breakpoints => _breakpoints;

    private readonly BreakpointList _breakpoints;

    private class CurrentOperationMode(ILogger<ScriptDebugger> logger, bool stopAtNextBlock, Context? stopAtParent = null, IScriptBreakpoint? stopAtBlock = null, IScriptSite? stopAtScript = null)
    {
        /// <summary>
        /// See if we should stop at the current context - can
        /// be either step into, step out, step over or stop at.
        /// </summary>
        /// <param name="context">Current execution context.</param>
        /// <returns>Set if we should stop execution.</returns>
        public bool MustStop(ScriptDebugContext context)
        {
            /* Single step mode. */
            if (stopAtNextBlock)
            {
                logger.LogTrace("Stop in single step mode at {Breakpoint}", context.Position);

                return true;
            }

            /* Stop at temporary position either through step over or run at. */
            if (stopAtBlock == context.Position)
            {
                logger.LogTrace("Stop on temporary breakpoint at {Breakpoint}", context.Position);

                return true;
            }

            /* Left a nested script. */
            if (stopAtScript == context.Context.Engine)
            {
                logger.LogTrace("Stepped out of nested script at {Breakpoint}", context.Position);

                return true;
            }

            /* Step out. */
            for (var test = stopAtParent; test != null; test = test.Parent)
                if (context.Context == test)
                {
                    logger.LogTrace("Stepped out of procedure at {Breakpoint}", context.Position);

                    return true;
                }

            return false;
        }
    }

    private CurrentOperationMode _operationMode;

    /// <summary>
    /// Current execution context.
    /// </summary>
    public ScriptDebugContext? StoppedAt => _context;

    private ScriptDebugContext? _context = null;

    /// <inheritdoc/>
    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (value == _enabled) return;

            _enabled = value;

            Restart();

            SomethingChanged();
        }
    }

    private bool _enabled = false;

    /// <inheritdoc/>
    public IScriptPosition? CurrentPosition => _context;

    /// <summary>
    /// Helper to manage engine stops.
    /// </summary>
    private TaskCompletionSource _stop = new();

    /// <summary>
    /// Initialize this debugger instance.
    /// </summary>
    /// <param name="logger">Logging helper.</param>
    public ScriptDebugger(ILogger<ScriptDebugger> logger)
    {
        Logger = logger;

        _breakpoints = new(this);
        _operationMode = new(Logger, false);
    }

    /// <inheritdoc/>
    public virtual void RunTo(string scriptId, string blockId)
    {
        var at = new ScriptBreakpoint(scriptId, blockId);

        Logger.LogTrace("Adding temporary breakpoint at {Breakpoint}", at);

        _operationMode = new(Logger, false, stopAtBlock: at);

        Restart();
    }

    /// <inheritdoc/>
    public virtual void Continue(ScriptDebugContinueModes mode)
    {
        var context = StoppedAt;

        switch (mode)
        {
            case ScriptDebugContinueModes.Normal:
                Logger.LogTrace("Continue regular execution of script");

                _operationMode = new(Logger, false);

                break;
            case ScriptDebugContinueModes.StepInto:
                Logger.LogTrace("Execute the current block and stop immediatly after");

                _operationMode = new(Logger, true);

                break;
            case ScriptDebugContinueModes.StepOver:
                Logger.LogTrace("Step over the current block and stop on the next");

                if (context == null) throw new InvalidOperationException("debugger not active");

                if (context.Block.Next == null) goto case ScriptDebugContinueModes.StepOut;

                _operationMode = new(Logger, false, stopAtBlock: new ScriptBreakpoint(context.ScriptId, context.Block.Next.Id));

                break;
            case ScriptDebugContinueModes.StepOut:
                Logger.LogTrace("Execute the current prodecure and stop after the return");

                if (context == null) throw new InvalidOperationException("debugger not active");

                if (context.Context.Parent == null && context.Context.Engine.ParentSite != null) goto case ScriptDebugContinueModes.LeaveNested;

                _operationMode = new(Logger, false, stopAtParent: context.Context.Parent);

                break;
            case ScriptDebugContinueModes.LeaveNested:
                Logger.LogTrace("Execute the current script and stop after if finishes");

                if (context == null) throw new InvalidOperationException("debugger not active");

                if (context.Context.Engine.ParentSite == null)
                    throw new InvalidOperationException("not a nested script");

                _operationMode = new(Logger, false, stopAtScript: context.Context.Engine.ParentSite);

                break;
            default:
                throw new ArgumentException($"Unsupported debug mode {mode}", nameof(mode));
        }

        Restart();
    }

    /// <summary>
    /// Install an operating context.
    /// </summary>
    /// <param name="block">Current block.</param>
    /// <param name="context">Current execution context.</param>
    /// <param name="reason">Reason for the debugger to be called.</param>
    /// <param name="handler">What to do.</param>
    private async Task<T> RunAsync<T>(Block block, Context context, ScriptDebuggerStopReason reason, Func<ScriptDebugContext, Task<T>> handler)
    {
        /* We are not active. */
        if (!Enabled) return default!;

        /* Ignore all blocky internal warmup blocks. */
        if (block.Type == null)
        {
            Logger.LogTrace("Skipping hidden initializer block");

            return default!;
        }

        /* Must be a well known script. */
        if (context.Engine.CurrentScript is not IGenericScript script || string.IsNullOrEmpty(script.Request.ScriptId)) return default!;

        /* In parallel mode a single break stops anything. */
        using (_sync.Wait())
        {
            /* Simplify overloads by providing some execution context. */
            _context = new(script.Request.ScriptId, block, reason, context, this);

            SomethingChanged();

            try
            {
                return await handler(_context);
            }
            finally
            {
                _context = null;

                SomethingChanged();
            }
        }
    }

    /// <inheritdoc/>
    public virtual Task InterceptAsync(Block block, Context context, ScriptDebuggerStopReason reason)
        => RunAsync(block, context, reason, async stoppedAt =>
            {
                switch (reason)
                {
                    case ScriptDebuggerStopReason.Enter:
                        {
                            var regular = _operationMode.MustStop(stoppedAt);

                            if (regular || _breakpoints[stoppedAt.ScriptId, stoppedAt.BlockId]?.Enabled == true)
                            {
                                if (!regular) Logger.LogTrace("Stop at breakpoint {Breakpoint}", stoppedAt.Position);

                                Continue(ScriptDebugContinueModes.Normal);

                                await OnBreakAsync();

                                return true;
                            }
                        }

                        break;
                    case ScriptDebuggerStopReason.Leave:
                        {
                            if (_breakpoints.BreakOnEndOfScript)
                            {
                                Logger.LogTrace("Stop at end of script {Breakpoint}", stoppedAt.Position);

                                await OnBreakAsync();

                                return true;
                            }

                            break;
                        }
                }

                return false;
            });

    /// <summary>
    /// Called when we stopped for some reason other than a breakpoint or an exception.
    /// </summary>
    protected virtual Task OnBreakAsync() => Task.CompletedTask;

    /// <summary>
    /// Process incoming exception.
    /// </summary>
    /// <param name="original">Exception seen</param>
    /// <returns>Exception to process, null to ignoree</returns>
    protected virtual Task<Exception?> OnExceptionDetectedAsync(Exception original) => Task.FromResult<Exception?>(original);

    /// <inheritdoc/>
    public virtual void ScriptFinished(Exception? exception)
    {
    }

    /// <inheritdoc/>
    public List<ScriptDebugVariableScope>? GetVariables() => StoppedAt?.GetVariables();

    /// <inheritdoc/>
    public virtual Task<Exception?> InterceptExceptionAsync(Block block, Context context, Exception original)
        => _breakpoints.BreakOnExceptions
            ? RunAsync(block, context, ScriptDebuggerStopReason.Exception, (stoppedAt) =>
            {
                Logger.LogTrace("Stop at exception {Exception}", original.Message);

                return OnExceptionDetectedAsync(original);
            })
            : Task.FromResult<Exception?>(original);

    /// <summary>
    /// Stop execution and wait.
    /// </summary>
    protected virtual Task StopAsync()
    {
        Logger.LogTrace("Script execution paused");

        var newStopper = new TaskCompletionSource();

        var stop = Interlocked.Exchange(ref _stop, newStopper);

        if (!stop.Task.IsCompleted) stop.SetCanceled();

        return newStopper.Task;
    }

    /// <summary>
    /// Restart regular execution.
    /// </summary>
    /// <param name="disposing">Set when restart is called during dispose.</param>
    protected virtual void Restart(bool disposing = false)
    {
        if (!disposing) Logger.LogTrace("Script execution resumed");

        var stop = _stop;

        if (!stop.Task.IsCompleted) _stop.SetResult();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Logger.LogTrace("Shutting down script debugger");

        Restart(true);

        _sync.Dispose();
    }

    /// <summary>
    /// Report that something changes.
    /// </summary>
    protected virtual void OnReportChange() { }

    internal void SomethingChanged() => OnReportChange();
}