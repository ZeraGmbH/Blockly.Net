using BlocklyNet.Scripting.Engine;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlocklyNet.Scripting.Logging;

/// <summary>
/// Log entry for a new single script.
/// </summary>
public class ScriptLoggingResult
{
    /// <summary>
    /// Name of the corresponding script if any.
    /// </summary>
    public string? ScriptName { get; set; }

    /// <summary>
    /// Unique identifier of the corresponding script if any.
    /// </summary>
    public string? ScriptId { get; set; }

    /// <summary>
    /// Outcome of the script.
    /// </summary>
    [NotNull, Required]
    public ScriptExecutionResultTypes Result { get; set; }

    /// <summary>
    /// When the script was started.
    /// </summary>
    [NotNull, Required]
    public DateTime Started { get; set; }

    /// <summary>
    /// When the script finished.
    /// </summary>
    public DateTime? Finished { get; set; }

    /// <summary>
    /// Opaque details of the script according to the type.
    /// </summary>
    [NotNull, Required]
    public string? Details { get; set; }

    /// <summary>
    /// Nested scripts initiated by this script.
    /// </summary>
    [NotNull, Required]
    public List<string> Children { get; set; } = [];

    /// <summary>
    /// Information on finished execution groups.
    /// </summary>
    public ScriptGroupStatus? GroupsFinished { get; set; }
}
