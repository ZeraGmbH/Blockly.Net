using System.Collections;
using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;
using BlocklyNet.Scripting;
using BlocklyNet.Scripting.Definition;
using BlocklyNet.Scripting.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace BlocklyNet.Extensions;

/// <summary>
/// Run a user defined script.
/// </summary>
[CustomBlock(
  "run_script_by_name",
  "Scripts",
  @"{
      ""message0"": ""RunScript %1 Display name %2 Id %3 Parameters %4 Do not execute %5"",
      ""args0"": [
          {
            ""type"": ""input_dummy""
          },
          {
            ""type"": ""input_value"",
            ""name"": ""NAME"",
            ""check"": ""String""
          },
          {
            ""type"": ""input_value"",
            ""name"": ""ID"",
            ""check"": ""String""
          },
          {
            ""type"": ""input_value"",
            ""name"": ""ARGS"",
            ""check"": [""Array(run_script_parameter)"", ""Array""]
          },
          {
            ""type"": ""input_value"",
            ""name"": ""BUILDONLY"",
            ""check"": ""Boolean""
          }
      ],
      ""output"": null,
      ""colour"": 230,
      ""tooltip"": ""Run a script by name"",
      ""helpUrl"": """"
  }",
  @"{
    ""inputs"": {
      ""NAME"": {
        ""shadow"": {
          ""type"": ""text"",
          ""fields"": {
            ""TEXT"": """"
          }
        }
      },
      ""ID"": {
        ""shadow"": {
          ""type"": ""text"",
          ""fields"": {
            ""TEXT"": """"
          }
        }
      },
      ""BUILDONLY"": {
        ""shadow"": {
          ""type"": ""logic_boolean"",
          ""fields"": {
            ""BOOL"": ""FALSE""
          }
        }
      }
    }
  }"
)]
public class RunScript : Block
{
  /// <summary>
  /// 
  /// </summary>
  /// <param name="context"></param>
  /// <returns></returns>
  private async Task<StartScript> ReadConfigurationAsync(Context context)
  {
    /* Find the script by its name - character casing is ignored. */
    var store = context.ServiceProvider.GetRequiredService<IScriptDefinitionStorage>();
    var byName = await Values.EvaluateAsync<string>("NAME", context);
    var byId = await Values.EvaluateAsync<string?>("ID", context, false);

    var script =
      (string.IsNullOrEmpty(byId) ? null : await store.GetAsync(byId))
      ?? await store.FindAsync(byName)
      ?? throw new ArgumentException($"script '{byName}' not found");

    /* Prepare to run generic script. */
    var config = context.ServiceProvider.GetService<IGenericScriptFactory>()?.Create() ?? new StartGenericScript();

    config.Name = script.Name;
    config.ScriptId = script.Id;
    config.ResultType = script.ResultType;

    /* Fill presets - just copy indicated variables with the same name. */
    var copies = await Values.EvaluateAsync<IEnumerable>("ARGS", context, false);

    if (copies != null)
      foreach (RunScriptParameter parameter in copies)
        config.Presets.Add(new() { Key = parameter.VariableName, Value = parameter.Value });

    return (StartScript)config;
  }

  /// <inheritdoc/>
  protected override async Task<object?> EvaluateAsync(Context context)
  {
    /* Convert the configuration. */
    var config = await ReadConfigurationAsync(context);

    /* We are prepared to be run in parallel to other scripts. */
    if (context.ParallelMode > 0) return config;

    /* Or we are hust building. */
    var buildOnly = await Values.EvaluateAsync<bool?>("BUILDONLY", context, false);

    if (buildOnly == true) return config;

    /* Run the script and report the result - in a new isolated environment. */
    var result = await context.Engine.RunAsync<GenericResult>(config);

    return result.Result;
  }
}