using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlocklyNet.Scripting.Generic;

/// <summary>
/// Describes a single variable preset.
/// </summary>
public class GenericScriptPreset
{
    /// <summary>
    /// Name of the variable.
    /// </summary>
    [NotNull, Required]
    public required string Key { get; set; }

    /// <summary>
    /// Value to use.
    /// </summary>
    public object? Value { get; set; }
}
