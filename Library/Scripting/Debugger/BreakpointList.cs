using Microsoft.Extensions.Logging;

namespace BlocklyNet.Scripting.Debugger;

/// <summary>
/// Management for the list of breakpoints. Implemention is thread-safe.
/// </summary>
internal class BreakpointList(ILogger<ScriptDebugger> logger) : IScriptBreakpoints
{
    /// <summary>
    /// Current list.
    /// </summary>
    private readonly Dictionary<ScriptBreakpoint, ScriptBreakpoint> _breakpoints = [];

    /// <inheritdoc/>
    public IScriptBreakpoint? this[string scriptId, string blockId]
    {
        get
        {
            lock (_breakpoints)
                return _breakpoints.TryGetValue(new ScriptBreakpoint(scriptId, blockId), out var bp) ? bp : null;
        }
    }

    /// <inheritdoc/>
    public bool BreakOnExceptions { get; set; }

    /// <inheritdoc/>
    public void Add(string scriptId, string blockId, string? description = null)
    {
        var bp = new ScriptBreakpoint(scriptId, blockId, description);

        lock (_breakpoints)
        {
            logger.LogTrace("{What} breakpoint {Breakpoint}", _breakpoints.ContainsKey(bp) ? "Updating" : "Adding", bp.ToString());

            _breakpoints[bp] = bp;
        }
    }

    /// <inheritdoc/>
    public IScriptBreakpoint[] GetAll()
    {
        lock (_breakpoints)
            return [.. _breakpoints.Values];
    }

    /// <inheritdoc/>
    public void Remove(string scriptId, string blockId)
    {
        var bp = new ScriptBreakpoint(scriptId, blockId);

        logger.LogTrace("Removing breakpoint {Breakpoint}", bp.ToString());

        lock (_breakpoints)
            _breakpoints.Remove(bp);
    }
}
