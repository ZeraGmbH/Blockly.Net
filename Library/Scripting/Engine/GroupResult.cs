using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// Describes the result of a execution group.
/// </summary>
public class GroupResult
{
    /// <summary>
    /// Execution state of the group.
    /// </summary>
    [NotNull, Required]
    public GroupResultType Type { get; set; } = GroupResultType.Invalid;

    /// <summary>
    /// Result of the execution - available whenever the type is not running.
    /// </summary>
    /// <value></value>
    public object? Result { get; set; }
}