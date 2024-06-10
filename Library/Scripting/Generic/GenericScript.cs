using System.Text.Json;
using BlocklyNet.Extensions.Builder;
using BlocklyNet.Scripting.Definition;
using BlocklyNet.Scripting.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace BlocklyNet.Scripting.Generic;

/// <summary>
/// 
/// /// </summary>
public interface IGenericScript
{
    /// <summary>
    /// 
    /// </summary>
    IStartGenericScript Request { get; }
}

/// <summary>
/// Algorithm to execute an error measurement.
/// </summary>
/// <param name="request">All parameters for the measurement.</param>
/// <param name="engine">Script engine executing the measurement.</param>
/// <param name="options">Additional configuration of the script run-time.</param>
public class GenericScript(StartGenericScript request, IScriptSite engine, StartScriptOptions? options)
    : Script<StartGenericScript, GenericResult, StartScriptOptions>(request, engine, options), IGenericScript
{
    IStartGenericScript IGenericScript.Request => Request;

    /// <inheritdoc/>
    protected override Task OnExecute() => Execute(this);

    public static async Task Execute<TRequest, TOptions>(Script<TRequest, GenericResult, TOptions> script, Action? afterPresets = null)
        where TRequest : StartScript, IStartGenericScript
        where TOptions : StartScriptOptions
    {
        /* Translate parameters. */
        var presets = script.Request.Presets.ToDictionary(p => p.Key, p => (p.Value is JsonElement json) ? json.ToJsonScalar() : p.Value);

        /* Prepare for logging. */
        script.Request.Presets = presets.Select(d => new GenericScriptPreset { Key = d.Key, Value = d.Value }).ToList();

        afterPresets?.Invoke();

        /* Find the script. */
        var di = script.Engine.ServiceProvider;
        var def = await di.GetRequiredService<IScriptDefinitionStorage>().Get(script.Request.ScriptId) ?? throw new ArgumentException("Script not found.");

        /* Validate presets. */
        var models = di.GetRequiredService<IScriptModels>();

        foreach (var param in def.Parameters)
        {
            /* Check requirement. */
            presets.TryGetValue(param.Name, out var preset);

            if (param.Required == true && preset == null)
                throw new ArgumentException($"Required parameter {param.Name} not set.");

            /* See if conversion is possible. */
            if (preset is string enumString)
            {
                if (!models.Enums.TryGetValue(param.Type, out var enumInfo)) continue;

                presets[param.Name] = Enum.Parse(enumInfo.Type, enumString);
            }
            else if (preset is JsonElement json)
            {
                /* Check for converter. */
                if (!models.Models.TryGetValue(param.Type, out var modelInfo)) continue;

                /* Do convert if we get raw json. */
                presets[param.Name] = JsonSerializer.Deserialize(json.ToString(), modelInfo.Type, JsonUtils.JsonSettings);
            }
        }

        /* Execute the script. */
        var result = await script.Engine.Evaluate(def.Code, presets);

        /* Report variables as a result. */
        var results = new GenericResult { Result = result, ResultType = script.Request.ResultType, ScriptId = script.Request.ScriptId };

        script.SetResult(results);
    }
}
