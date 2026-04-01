using Microsoft.Extensions.Logging;

namespace BlocklyNet.Scripting.Debugger;

/// <summary>
/// Management for the list of breakpoints. Implemention is thread-safe.
/// </summary>
internal class BreakpointList(ScriptDebugger debugger) : IScriptBreakpoints
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
    public bool BreakOnExceptions
    {
        get => _breakOnExceptions;
        set
        {
            if (_breakOnExceptions == value) return;

            _breakOnExceptions = value;

            debugger.SomethingChanged();
        }
    }

    private bool _breakOnExceptions;

    /// <inheritdoc/>
    public bool BreakOnEndOfScript
    {
        get => _breakOnEndOfScript;
        set
        {
            if (_breakOnEndOfScript == value) return;

            _breakOnEndOfScript = value;

            debugger.SomethingChanged();
        }
    }

    private bool _breakOnEndOfScript;

    /// <inheritdoc/>
    public void Add(string scriptId, string blockId, string? description = null)
    {
        var bp = new ScriptBreakpoint(scriptId, blockId, description, debugger);

        lock (_breakpoints)
        {
            debugger.InternalLogger.LogTrace("{What} breakpoint {Breakpoint}", _breakpoints.ContainsKey(bp) ? "Updating" : "Adding", bp.ToString());

            _breakpoints[bp] = bp;
        }

        debugger.SomethingChanged();
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

        debugger.InternalLogger.LogTrace("Removing breakpoint {Breakpoint}", bp.ToString());

        lock (_breakpoints)
            if (!_breakpoints.Remove(bp))
                return;

        debugger.SomethingChanged();
    }
}
