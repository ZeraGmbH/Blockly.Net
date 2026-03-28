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
    }

    /// <summary>
    /// All active breakpoints.
    /// </summary>
    public IScriptBreakpoints Breakpoints => _breakpoints;

    private readonly BreakpointList _breakpoints = new();

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
                                StopOnStart = false;

                                await OnFirstBlockAsync();
                            }
                            else if (SingleStep)
                                await OnSingleStepAsync();
                            else
                            {
                                var hit = _breakpoints[script.Request.ScriptId, block.Id];

                                if (hit != null) await OnBreakpointHitAsync(hit);
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

    /// <inheritdoc/>
    public virtual void ScriptFinished(Exception? exception)
    {
    }
}