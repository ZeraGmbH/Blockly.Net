namespace BlocklyNet.Scripting.Debugger;

/// <summary>
/// A list of breakpoints.
/// </summary>
public interface IScriptBreakpoints
{
    /// <summary>
    /// Add a breakpoint.
    /// </summary>
    /// <param name="scriptId">Script identifier.</param>
    /// <param name="blockId">Block identifier in block.</param>
    void Add(string scriptId, string blockId);

    /// <summary>
    /// Remove ALL breakpoints defined on a single block.
    /// </summary>
    /// <param name="scriptId">Script identifier.</param>
    /// <param name="blockId">Block identifier in block.</param>
    void Remove(string scriptId, string blockId);

    /// <summary>
    /// Lookup a breakpoint.
    /// </summary>
    /// <param name="scriptId">Script identifier.</param>
    /// <param name="blockId">Block identifier in block.</param>
    IScriptBreakpoint? this[string scriptId, string blockId] { get; }
}
