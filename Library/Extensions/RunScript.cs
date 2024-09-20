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
      ""message0"": ""RunScript %1 %2 %3 %4 %5"",
      ""args0"": [
          {
            ""type"": ""input_dummy""
          },
          {
              ""type"": ""field_label_serializable"",
              ""name"": ""NAME"",
              ""text"": ""Display name""
          },
          {
            ""type"": ""input_value"",
            ""name"": ""NAME"",
            ""check"": ""String""
          },
          {
              ""type"": ""field_label_serializable"",
              ""name"": ""ARGS"",
              ""text"": ""Parameters""
          },
          {
            ""type"": ""input_value"",
            ""name"": ""ARGS"",
            ""check"": [""Array(run_script_parameter)"", ""Array""]
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
  public async Task<StartGenericScript> ReadConfigurationAsync(Context context)
  {
    /* Find the script by its name - character casing is ignored. */
    var store = context.ServiceProvider.GetRequiredService<IScriptDefinitionStorage>();
    var byName = await Values.EvaluateAsync<string>("NAME", context);
    var script = await store.FindAsync(byName) ?? throw new ArgumentException($"script '{byName}' not found");

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

    return (StartGenericScript)config;
  }

  /// <inheritdoc/>
  public override async Task<object?> EvaluateAsync(Context context)
  {
    /* We are prepared to be run in parallel to other scripts. */
    if (context.ParallelMode > 0) return this;

    /* Run the script and report the result - in a new isolated environment. */
    var result = await context.Engine.RunAsync<GenericResult, StartGenericScript>(await ReadConfigurationAsync(context));

    return result.Result;
  }
}