using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// Describes the script group execution status.
/// </summary>
public class ScriptGroupStatus<TStatus> where TStatus : GroupStatus<TStatus>
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
    public List<TStatus> GroupStatus { get; set; } = [];
}

/// <summary>
/// 
/// </summary>
public class ScriptGroupStatus : ScriptGroupStatus<GroupStatus>
{
    /// <summary>
    /// Set if a pause request has been detected.
    /// </summary>
    public bool HasBeenPaused { get; set; }
}