using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using BlocklyNet.Scripting.Engine;

namespace BlocklyNet.Scripting.Logging;

/// <summary>
/// Log entry for a new single measurement.
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
    /// Outcome of the measurement.
    /// </summary>
    [NotNull, Required]
    public required ScriptExecutionResultTypes Result { get; set; }

    /// <summary>
    /// When the measurement was started.
    /// </summary>
    [NotNull, Required]
    public required DateTime Started { get; set; }

    /// <summary>
    /// When the measurement finished.
    /// </summary>
    public DateTime? Finished { get; set; }

    /// <summary>
    /// Opaque details of the measurement according to the type.
    /// </summary>
    [NotNull, Required]
    public required string? Details { get; set; }

    /// <summary>
    /// Measurements initiated by this measurement.
    /// </summary>
    [NotNull, Required]
    public List<string> Children { get; set; } = [];

    /// <summary>
    /// Information on finished execution groups.
    /// </summary>
    public ScriptGroupStatus? GroupsFinished { get; set; }
}
