using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;
using Microsoft.Extensions.DependencyInjection;

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
    /// Debugger associated with this scope.
    /// </summary>
    [JsonIgnore]
    internal ScriptDebugger? Debugger { get; set; }

    /// <summary>
    /// Corresponding script.
    /// </summary>
    [NotNull, Required]
    public string ScriptId { get; set; } = null!;

    /// <summary>
    /// Optional name of the related procedure.
    /// </summary>
    public string? Procedure { get; set; }

    /// <summary>
    /// All variables in this scope.
    /// </summary>
    [NotNull, Required]
    public List<ScriptDebugVariableInformation> Variables { get; set; } = [];

    /// <summary>
    /// Update a single variable.
    /// </summary>
    /// <param name="name">Name of the variable.</param>
    /// <param name="jsonValue">Value of the variable.</param>
    public void SetVariable(string name, string jsonValue)
    {
        /* Must habe a runtime context. */
        var variables = (Context?.Variables) ?? throw new InvalidOperationException("no runtime context available");

        /* Must known variable. */
        if (!variables.ContainsKey(name)) throw new ArgumentException($"no variable '{name}", nameof(name));

        /* No value at all. */
        if (string.IsNullOrEmpty(jsonValue))
            variables[name] = null;
        else
        {
            var untyped = JsonSerializer.Deserialize<JsonElement>(jsonValue, JsonUtils.JsonSettings);

            if (!Context.VariableTypes.TryGetValue(name, out var type) || string.IsNullOrEmpty(type))
                variables[name] = untyped.ToJsonScalar();
            else
            {
                var modelInfos = Context.ServiceProvider.GetRequiredService<IScriptModels>();

                /* Convert if this is a known type. */
                if (modelInfos.Models.TryGetValue(type, out var typeInfo) || modelInfos.Enums.TryGetValue(type, out typeInfo))
                    variables[name] = JsonSerializer.Deserialize(untyped, typeInfo.Type, JsonUtils.JsonSettings);
                else
                    variables[name] = untyped.ToJsonScalar();
            }
        }

        Debugger?.SomethingChanged();
    }
}
