using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using BlocklyNet.Core.Model;

namespace BlocklyNet.Scripting.Debugger;

/// <summary>
/// Single scope of variables during script execution.
/// </summary>
public class ScriptDebugVariableScope
{
    /// <summary>
    /// Context the variable is taken from.
    /// </summary>
    [JsonIgnore]
    internal Context? Context { get; set; }

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
