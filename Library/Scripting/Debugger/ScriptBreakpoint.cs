namespace BlocklyNet.Scripting.Debugger;

/// <summary>
/// Defines a single breakpoint.
/// </summary>
internal class ScriptBreakpoint(string scriptId, string blockId) : IScriptBreakpoint
{
    /// <inheritdoc/>
    public bool Enabled { get; set; } = true;

    /// <inheritdoc/>
    public string ScriptId { get; } = scriptId;

    /// <inheritdoc/>
    public string BlockId { get; } = blockId;

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is ScriptBreakpoint other && other.BlockId == BlockId && other.ScriptId == ScriptId;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(ScriptId, BlockId);
}
