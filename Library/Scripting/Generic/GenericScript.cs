using System.Text.Json;
using BlocklyNet.Extensions.Builder;
using BlocklyNet.Scripting.Definition;
using BlocklyNet.Scripting.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace BlocklyNet.Scripting.Generic;

/// <summary>
/// Algorithm to execute an error measurement.
/// </summary>
/// <param name="request">All parameters for the measurement.</param>
/// <param name="engine">Script engine executing the measurement.</param>
/// <param name="options">Additional configuration of the script run-time.</param>
public class GenericScript(StartGenericScript request, IScriptSite engine, StartScriptOptions? options) : Script<StartGenericScript, GenericResult>(request, engine, options)
{
    /// <inheritdoc/>
    protected override async Task OnExecute()
    {
        /* Translate parameters. */
        var presets = Request.Presets.ToDictionary(p => p.Key, p =>
        {
            if (p.Value is JsonElement json)
                switch (json.ValueKind)
                {
                    case JsonValueKind.String:
                        return json.Deserialize<string>();
                    case JsonValueKind.Number:
                        return json.Deserialize<double>();
                    case JsonValueKind.Null:
                        return null;
                    case JsonValueKind.True:
                        return true;
                    case JsonValueKind.False:
                        return false;
                }

            return p.Value;
        });

        /* Prepare for logging. */
        Request.Presets = presets.Select(d => new GenericScriptPreset { Key = d.Key, Value = d.Value }).ToList();

        /* Find the script. */
        var script = await Engine.ServiceProvider.GetRequiredService<IScriptDefinitionStorage>().Get(Request.ScriptId) ?? throw new ArgumentException("Script not found.");

        /* Validate presets. */
        var models = GetService<IScriptModels>();

        foreach (var param in script.Parameters)
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
        var result = await Engine.Evaluate(script.Code, presets);

        /* Report variables as a result. */
        var results = new GenericResult { Result = result, ResultType = Request.ResultType, ScriptId = Request.ScriptId };

        SetResult(results);
    }
}
