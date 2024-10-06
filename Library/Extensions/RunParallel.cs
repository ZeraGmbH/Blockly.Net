using System.Collections;
using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;
using BlocklyNet.Scripting;
using BlocklyNet.Scripting.Generic;

namespace BlocklyNet.Extensions;

/// <summary>
/// Run scripts in parallel.
/// </summary>
[CustomBlock(
  "run_script_in_parallel",
  "Scripts",
  @"{
      ""message0"": ""RunParallel %1 %2 %3 %4 %5"",
      ""args0"": [
          {
            ""type"": ""input_dummy""
          },
          {
              ""type"": ""field_label_serializable"",
              ""name"": ""SCRIPTS"",
              ""text"": ""scripts""
          },
          {
            ""type"": ""input_value"",
            ""name"": ""SCRIPTS"",
            ""check"": [""Array(run_script_by_name)"", ""Array""]
          },
          {
              ""type"": ""field_label_serializable"",
              ""name"": ""LEADINGSCRIPT"",
              ""text"": ""leading""
          },
          {
            ""type"": ""input_value"",
            ""name"": ""LEADINGSCRIPT"",
            ""check"": ""Number""
          }
      ],
      ""output"": null,
      ""colour"": 230,
      ""tooltip"": ""Run a a number of scripts in parallel"",
      ""helpUrl"": """"
  }",
  @""
)]
public class RunParallel : Block
{
  /// <inheritdoc/>
  public override async Task<object?> EvaluateAsync(Context context)
  {
    /* Request the script blocks in configuration mode - will not execute on this thread. */
    context.ParallelMode++;

    try
    {
      /* Load array of scripts. */
      var scripts = await Values.EvaluateAsync<IEnumerable>("SCRIPTS", context);
      var leading = await Values.EvaluateAsync<double?>("LEADINGSCRIPT", context, false);

      /* Request configuration for all scripts - allow empty array elements. */
      var configs = new List<StartScript>();

      foreach (RunScript script in scripts)
        configs.Add(await script.ReadConfigurationAsync(context));

      /* Lifetime control. */
      var leadingDone = false;

      /* Create separate tasks for each script. */
      var options = new StartScriptOptions { ShouldStopNow = () => leadingDone };
      var tasks = configs.Select(config => context.Engine.RunAsync<GenericResult>(config, options)).ToArray();

      /* Wait for the leading task to finish. */
      if (leading is double)
      {
        await tasks[((int)leading) - 1];

        /* Let other tasks stop as soon as possible. */
        leadingDone = true;
      }

      /* Wait for all other tasks to finish and get results */
      var results = await Task.WhenAll(tasks);

      /* Report combined results. */
      return results.Select(r => r.Result).ToArray();
    }
    finally
    {
      /* Switch this execution context back to evaluation mode. */
      context.ParallelMode--;
    }
  }
}