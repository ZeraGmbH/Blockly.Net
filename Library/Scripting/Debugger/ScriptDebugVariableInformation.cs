using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlocklyNet.Scripting.Debugger;

/// <summary>
/// Information on a single variable.
/// </summary>
public class ScriptDebugVariableInformation
{
    /// <summary>
    /// Name of the variable.
    /// </summary>
    [NotNull, Required]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Type of the variable.
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Value in JSON representation ready to be transferred.
    /// </summary>
    /// <value></value>
    public string? Value { get; set; }
}
