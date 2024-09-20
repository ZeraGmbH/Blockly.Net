using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// Describes an Out-of-Bound exception during script execution.
/// </summary>
public class ScriptError : ScriptInformationWithGroupStatus
{
    /// <summary>
    /// A string describing the error.
    /// </summary>
    [NotNull, Required]
    public string ErrorMessage { get; set; } = null!;
}
