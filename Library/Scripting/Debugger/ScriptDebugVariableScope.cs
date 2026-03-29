using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlocklyNet.Scripting.Debugger;

/// <summary>
/// Single scope of variables during script execution.
/// </summary>
public class ScriptDebugVariableScope
{
    /// <summary>
    /// Corresponding script.
    /// </summary>
    [NotNull, Required]
    public string ScriptId { get; set; } = null!;

    /// <summary>
    /// All variables in this scope.
    /// </summary>
    [NotNull, Required]
    public List<ScriptDebugVariableInformation> Variables { get; set; } = [];
}
