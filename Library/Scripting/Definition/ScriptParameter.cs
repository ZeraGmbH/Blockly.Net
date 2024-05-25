using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlocklyNet.Scripting.Definition;

/// <summary>
/// Parameter to a script a use can set before 
/// starting the script.
/// </summary>
public class ScriptParameter
{
    /// <summary>
    /// The type of the input.
    /// </summary>
    [Required, NotNull]
    public required string Type { get; set; }

    /// <summary>
    /// The name of the parameter.
    /// </summary>
    [Required, NotNull, MinLength(1)]
    public required string Name { get; set; }

    /// <summary>
    /// Set to indicate that the parameter is required.
    /// </summary>
    public bool? Required { get; set; }
}