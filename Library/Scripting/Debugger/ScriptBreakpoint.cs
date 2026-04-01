namespace BlocklyNet.Scripting.Debugger;

/// <summary>
/// Defines a single breakpoint.
/// </summary>
internal class ScriptBreakpoint(string scriptId, string blockId, string? description = null, ScriptDebugger? debugger = null) : IScriptBreakpoint
{
    /// <inheritdoc/>
    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (_enabled == value) return;

            _enabled = value;

            debugger?.SomethingChanged();
        }
    }

    private bool _enabled = true;

    /// <inheritdoc/>
    public string ScriptId { get; } = scriptId;

    /// <inheritdoc/>
    public string BlockId { get; } = blockId;

    /// <inheritdoc/>
    public string? Description
    {
        get => _description;
        set
        {
            if (_description == value) return;

            _description = value;

            debugger?.SomethingChanged();
        }
    }

    private string? _description = description;

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is ScriptBreakpoint other && other.BlockId == BlockId && other.ScriptId == ScriptId;

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(ScriptId, BlockId);

    /// <inheritdoc/>
    public override string ToString() => $"{(Enabled ? string.Empty : "-")}{BlockId}@{ScriptId}";
}
