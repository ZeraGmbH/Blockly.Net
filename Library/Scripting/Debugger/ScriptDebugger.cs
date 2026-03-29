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

        public void Add(string scriptId, string blockId)
        {
            var bp = new ScriptBreakpoint(scriptId, blockId);

            lock (_breakpoints)
                _breakpoints[bp] = bp;
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

    /// <inheritdoc/>
    public virtual async Task InterceptAsync(Block block, Context context, ScriptDebuggerStopReason reason)
    {
        /* Must be a well known script. */
        if (context.Engine.CurrentScript is not IGenericScript script || string.IsNullOrEmpty(script.Request.ScriptId)) return;

        /* Simplify overloads by providing some execution context. */
        _context = new(script.Request.ScriptId, block, reason, context);

        try
        {
            if (block.Type != null)
                switch (reason)
                {
                    /* Before execution a block check for a breakpoint. */
                    case ScriptDebuggerStopReason.Enter:
                        {
                            if (StopOnStart)
                            {
                                _volatile = null;

                                StopOnStart = false;

                                await OnFirstBlockAsync();

                                break;
                            }

                            var volatileBp = _volatile;

                            if (volatileBp != null && volatileBp.ScriptId == script.Request.ScriptId && volatileBp.BlockId == block.Id)
                            {
                                _volatile = null;

                                await OnVolatileStopAsync();

                                break;
                            }

                            if (SingleStep)
                            {
                                _volatile = null;

                                await OnSingleStepAsync();

                                break;
                            }

                            var hit = _breakpoints[script.Request.ScriptId, block.Id];

                            if (hit != null)
                            {
                                _volatile = null;

                                await OnBreakpointHitAsync(hit);

                                break;
                            }

                            break;
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

    /// <inheritdoc/>
    public virtual void ScriptFinished(Exception? exception)
    {
    }

    /// <inheritdoc/>
    public List<ScriptDebugVariableScope>? GetVariables() => Context?.GetVariables();
}