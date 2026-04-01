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
    /// <param name="description">Optional description to display to the user.</param>
    void Add(string scriptId, string blockId, string? description = null);

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

    /// <summary>
    /// Retrieve all breakpoints.
    /// </summary>
    IScriptBreakpoint[] GetAll();

    /// <summary>
    /// Set to break on each exception.
    /// </summary>
    bool BreakOnExceptions { get; set; }

    /// <summary>
    /// Set to break at the end of every script.
    /// </summary>
    bool BreakOnEndOfScript { get; set; }
}
