using BlocklyNet.Core.Model;
using BlocklyNet.Scripting.Generic;

namespace BlocklyNet.Scripting.Debugger;

/// <summary>
/// Helper class to implement debuggers.
/// </summary>
public abstract class ScriptDebugger : IScriptDebugger
{
    private class BreakpointList : IScriptBreakpoints
    {
        private readonly Dictionary<ScriptBreakpoint, ScriptBreakpoint> _breakpoints = [];

        public IScriptBreakpoint? this[string scriptId, string blockId]
        {
            get
            {
                lock (_breakpoints)
                    return _breakpoints.TryGetValue(new ScriptBreakpoint(scriptId, blockId), out var bp) ? bp : null;
            }
        }

        public bool BreakOnExceptions { get; set; }

        public void Add(string scriptId, string blockId, string? description = null)
        {
            var bp = new ScriptBreakpoint(scriptId, blockId, description);

            lock (_breakpoints)
                _breakpoints[bp] = bp;
        }

        public IScriptBreakpoint[] GetAll()
        {
            lock (_breakpoints)
                return [.. _breakpoints.Values];
        }

        public void Remove(string scriptId, string blockId)
        {
            lock (_breakpoints)
                _breakpoints.Remove(new ScriptBreakpoint(scriptId, blockId));
        }
    }

    /// <summary>
    /// All active breakpoints.
    /// </summary>
    public IScriptBreakpoints Breakpoints => _breakpoints;

    private readonly BreakpointList _breakpoints = new();

    private class CurrentOperationMode(bool stopAtNextBlock, Context? stopAtParent = null, IScriptBreakpoint? stopAtBlock = null, string? stopAtScript = null)
    {
        /// <summary>
        /// See if we should stop at the current context - can
        /// be either step into, step out, step over or stop at.
        /// </summary>
        /// <param name="context">Current execution context.</param>
        /// <returns>Set if we should stop execution.</returns>
        public bool MustStop(ScriptDebugContext context)
        {
            /* Step into, step over or stop at. */
            if (stopAtNextBlock || stopAtBlock == context.Position || stopAtScript == context.ScriptId)
                return true;

            /* Step out. */
            for (var test = stopAtParent; test != null; test = test.Parent)
                if (context.Context == test)
                    return true;

            return false;
        }
    }

    private CurrentOperationMode _operationMode = new(false);

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
        }
    }

    private bool _enabled = false;

    /// <inheritdoc/>
    public IScriptPosition? CurrentPosition => _context;

    /// <summary>
    /// Helper to manage engine stops.
    /// </summary>
    private TaskCompletionSource _stop = new();

    /// <inheritdoc/>
    public virtual void RunTo(string scriptId, string blockId)
    {
        _operationMode = new(false, stopAtBlock: new ScriptBreakpoint(scriptId, blockId));

        Restart();
    }

    /// <inheritdoc/>
    public virtual void Continue(ScriptDebugContinueModes mode)
    {
        var context = StoppedAt;

        switch (mode)
        {
            case ScriptDebugContinueModes.Normal:
                _operationMode = new(false);
                break;
            case ScriptDebugContinueModes.StepInto:
                _operationMode = new(true);
                break;
            case ScriptDebugContinueModes.StepOver:
                if (context == null) throw new InvalidOperationException("debugger not active");

                if (context.Block.Next == null)
                    _operationMode = new(false);
                else
                    _operationMode = new(false, stopAtBlock: new ScriptBreakpoint(context.ScriptId, context.Block.Next.Id));

                break;
            case ScriptDebugContinueModes.StepOut:
                if (context == null) throw new InvalidOperationException("debugger not active");

                _operationMode = new(false, stopAtParent: context.Context.Parent);

                break;
            case ScriptDebugContinueModes.LeaveNested:
                if (context == null) throw new InvalidOperationException("debugger not active");

                if (context.Context.Engine.ParentSite?.CurrentScript is not IGenericScript script || string.IsNullOrEmpty(script.Request.ScriptId))
                    throw new InvalidOperationException("not a nested script");

                _operationMode = new(false, stopAtScript: script.Request.ScriptId);

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
    /// <returns>Set if processing should continue.</returns>
    private ScriptDebugContext? CreateContext(Block block, Context context, ScriptDebuggerStopReason reason)
    {
        /* We are not active. */
        if (!Enabled) return null;

        /* Ignore all blocky internal warmup blocks. */
        if (block.Type == null) return null;

        /* Must be a well known script. */
        if (context.Engine.CurrentScript is not IGenericScript script || string.IsNullOrEmpty(script.Request.ScriptId)) return null;

        /* Simplify overloads by providing some execution context. */
        _context = new(script.Request.ScriptId, block, reason, context);

        return _context;
    }

    /// <inheritdoc/>
    public virtual async Task InterceptAsync(Block block, Context context, ScriptDebuggerStopReason reason)
    {
        /* See if we could handle the exception. */
        var stoppedAt = CreateContext(block, context, reason);

        if (stoppedAt == null) return;

        try
        {
            /* Before execution a block check for a breakpoint. */
            if (reason == ScriptDebuggerStopReason.Enter)
            {
                /* Volatile stop or active breakpoint hit. */
                if (_operationMode.MustStop(stoppedAt) || _breakpoints[stoppedAt.ScriptId, stoppedAt.BlockId]?.Enabled == true)
                {
                    Continue(ScriptDebugContinueModes.Normal);

                    await OnBreakAsync();
                }
            }
        }
        finally
        {
            /* This inspection is finished - get rid of context. */
            _context = null;
        }
    }

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
    public virtual async Task<Exception?> InterceptExceptionAsync(Block block, Context context, Exception original)
    {
        /* No interested in exceptions. */
        if (!_breakpoints.BreakOnExceptions) return original;

        /* See if we could handle the exception. */
        if (CreateContext(block, context, ScriptDebuggerStopReason.Exception) == null) return original;

        try
        {
            return await OnExceptionDetectedAsync(original);
        }
        finally
        {
            /* This inspection is finished - get rid of context. */
            _context = null;
        }
    }

    /// <summary>
    /// Stop execution and wait.
    /// </summary>
    protected Task StopAsync()
    {
        var newStopper = new TaskCompletionSource();

        Interlocked.Exchange(ref _stop, newStopper)?.SetCanceled();

        return newStopper.Task;
    }

    /// <summary>
    /// Restart regular execution.
    /// </summary>
    protected void Restart()
    {
        var stop = _stop;

        if (stop.Task.IsCompleted) _stop.SetResult();
    }
}