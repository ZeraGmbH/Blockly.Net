using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlocklyNet.Extensions.Builder;

/// <summary>
/// Describe a model block.
/// </summary>
public class ScriptEngineModelInfo
{
    /// <summary>
    /// Display name of the type.
    /// </summary>
    [NotNull, Required]
    public required string Name { get; set; }

    /// <summary>
    /// Unique key of the type.
    /// </summary>
    [NotNull, Required]
    public required string Type { get; set; }
}

