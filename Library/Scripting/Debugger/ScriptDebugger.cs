using BlocklyNet.Core.Model;
using BlocklyNet.Scripting.Generic;

namespace BlocklyNet.Scripting.Debugger;

/// <summary>
/// Helper class to implement debuggers.
/// </summary>
public abstract class ScriptDebugger : IScriptDebugger
{
    private class BreakpointList(ScriptDebugger debugger) : IScriptBreakpoints
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

        public void Add(string scriptId, string blockId)
        {
            var bp = new ScriptBreakpoint(scriptId, blockId);

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

        public void RunTo(string scriptId, string blockId) => debugger.RunTo(scriptId, blockId);
    }

    /// <summary>
    /// All active breakpoints.
    /// </summary>
    public IScriptBreakpoints Breakpoints => _breakpoints;

    private readonly BreakpointList _breakpoints;

    /// <summary>
    /// Current execution context.
    /// </summary>
    protected ScriptDebugContext Context => _context;

    private ScriptDebugContext _context = null!;

    /// <summary>
    /// Single step each block.
    /// </summary>
    public bool SingleStep { get; set; } = false;

    /// <summary>
    /// Stop on first real block.
    /// </summary>
    public bool StopOnStart { get; set; } = false;

    /// <inheritdoc/>
    public bool Enabled { get; set; } = false;

    /// <inheritdoc/>
    public IScriptPosition? CurrentPosition => _context;

    /// <summary>
    /// Volatile breakpoint used for step over and run to block.
    /// </summary>
    private IScriptBreakpoint? _volatile;

    /// <summary>
    /// Initialize a new debugger.
    /// </summary>
    protected ScriptDebugger()
    {
        _breakpoints = new(this);
    }

    /// <summary>
    /// Stop at the indicated block.
    /// </summary>
    /// <param name="scriptId">Script to use.</param>
    /// <param name="blockId">Block to stop at.</param>
    private void RunTo(string scriptId, string blockId) => _volatile = new ScriptBreakpoint(scriptId, blockId);

    /// <summary>
    /// Install an operating context.
    /// </summary>
    /// <param name="block">Current block.</param>
    /// <param name="context">Current execution context.</param>
    /// <param name="reason">Reason for the debugger to be called.</param>
    /// <returns>Set if processing should continue.</returns>
    private bool CreateContext(Block block, Context context, ScriptDebuggerStopReason reason)
    {
        /* We are not active. */
        if (!Enabled) return false;

        /* Ignore all blocky internal warmup blocks. */
        if (block.Type == null) return false;

        /* Must be a well known script. */
        if (context.Engine.CurrentScript is not IGenericScript script || string.IsNullOrEmpty(script.Request.ScriptId)) return false;

        /* Simplify overloads by providing some execution context. */
        _context = new(script.Request.ScriptId, block, reason, context);

        return true;
    }

    /// <inheritdoc/>
    public async Task InterceptAsync(Block block, Context context, ScriptDebuggerStopReason reason)
    {
        /* See if we could handle the exception. */
        if (!CreateContext(block, context, reason)) return;

        try
        {
            /* Before execution a block check for a breakpoint. */
            if (reason == ScriptDebuggerStopReason.Enter)
            {
                if (StopOnStart)
                {
                    _volatile = null;

                    StopOnStart = false;

                    await OnFirstBlockAsync();

                    return;
                }

                var volatileBp = _volatile;

                if (volatileBp != null && volatileBp.ScriptId == _context.ScriptId && volatileBp.BlockId == block.Id)
                {
                    _volatile = null;

                    await OnVolatileStopAsync();

                    return;
                }

                if (SingleStep)
                {
                    _volatile = null;

                    await OnSingleStepAsync();

                    return;
                }

                var hit = _breakpoints[_context.ScriptId, block.Id];

                if (hit != null)
                {
                    _volatile = null;

                    await OnBreakpointHitAsync(hit);

                    return;
                }
            }
        }
        finally
        {
            /* This inspection is finished - get rid of context. */
            _context = null!;
        }
    }

    /// <summary>
    /// Called when a breakpoint hit is detected.
    /// </summary>
    /// <param name="bp">Breakpoint hit.</param>
    protected virtual Task OnBreakpointHitAsync(IScriptBreakpoint bp) => Task.CompletedTask;

    /// <summary>
    /// Called in single step mode.
    /// </summary>
    protected virtual Task OnSingleStepAsync() => Task.CompletedTask;

    /// <summary>
    /// Called on the first block of a script.
    /// </summary>
    protected virtual Task OnFirstBlockAsync() => Task.CompletedTask;

    /// <summary>
    /// Hit some volatile breakpoint.
    /// </summary>
    protected virtual Task OnVolatileStopAsync() => Task.CompletedTask;

    /// <summary>
    /// Process incoming exception.
    /// </summary>
    /// <param name="original">Exception seen</param>
    /// <returns>Exception to process, null to ignoree</returns>
    protected virtual Task<Exception?> OnExceptionDetectedAsync(Exception original) => Task.FromResult<Exception?>(original);

    /// <inheritdoc/>
    public void ScriptFinished(Exception? exception)
    {
    }

    /// <inheritdoc/>
    public List<ScriptDebugVariableScope>? GetVariables() => Context?.GetVariables();

    /// <inheritdoc/>
    public async Task<Exception?> InterceptExceptionAsync(Block block, Context context, Exception original)
    {
        /* No interested in exceptions. */
        if (!_breakpoints.BreakOnExceptions) return original;

        /* See if we could handle the exception. */
        if (!CreateContext(block, context, ScriptDebuggerStopReason.Exception)) return original;

        try
        {
            return await OnExceptionDetectedAsync(original);
        }
        finally
        {
            /* This inspection is finished - get rid of context. */
            _context = null!;
        }
    }
}