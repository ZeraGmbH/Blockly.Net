using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// Progress of an active script.
/// </summary>
public class ScriptProgress
{
    /// <summary>
    /// The unique identifier of the active script.
    /// </summary>
    [Required, NotNull]
    public string JobId { get; set; } = null!;

    /// <summary>
    /// The progress information depending on the type of script.
    /// </summary>
    [Required, NotNull]
    public object Info { get; set; } = null!;

    /// <summary>
    /// Secondary progress - order by nesting level and the start of 
    /// child script.
    /// </summary>
    [Required, NotNull]
    public List<ProgressDetails> AllProgress { get; set; } = [];

    /// <summary>
    /// Information on the execution group status.
    /// </summary>
    [NotNull, Required]
    public List<GroupStatus> GroupStatus { get; set; } = [];
}
