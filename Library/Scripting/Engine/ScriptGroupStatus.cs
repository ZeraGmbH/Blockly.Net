using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// Describes the script group execution status.
/// </summary>
public class ScriptGroupStatus
{
    /// <summary>
    /// SHA256 hash of the script code executed.
    /// </summary>
    [Required, NotNull]
    public string CodeHash { get; set; } = null!;

    /// <summary>
    /// All the individual executed groups.
    /// </summary>
    [Required, NotNull]
    public List<GroupStatus> GroupStatus { get; set; } = [];
}