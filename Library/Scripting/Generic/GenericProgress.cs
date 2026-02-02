using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlocklyNet.Scripting.Generic;

/// <summary>
/// Progress information when running a script.
/// </summary>
public class GenericProgress
{
    /// <summary>
    /// Current progress in percentage between 0 and 100.
    /// </summary>
    [NotNull, Required]
    public required double Percentage { get; set; }

    /// <summary>
    /// Some extra data.
    /// </summary>
    public object? Payload { get; set; }

    /// <summary>
    /// Type of the payload to allow client to display it correctly.
    /// </summary>
    public string? PayloadType { get; set; }

    /// <summary>
    /// The script currently executing.
    /// </summary>
    [Required, NotNull]
    public string? ScriptId { get; set; }

    /// <summary>
    /// Set to hide this progress from the frontend.
    /// </summary>
    public bool? NoVisualisation { get; set; }
}