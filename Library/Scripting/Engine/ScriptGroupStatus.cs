namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// Describes the script group execution status.
/// </summary>
public class ScriptGroupStatus
{
    /// <summary>
    /// SHA256 hash of the script code executed.
    /// </summary>
    public string CodeHash { get; set; } = null!;

    /// <summary>
    /// All the individual executed groups.
    /// </summary>
    public List<GroupStatus> GroupStatus { get; set; } = [];
}