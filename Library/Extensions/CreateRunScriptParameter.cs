using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;

namespace BlocklyNet.Extensions;

/// <summary>
/// Describes a parameter of a script.
/// </summary>
public class RunScriptParameter
{
  /// <summary>
  /// Name of the variable to set.
  /// </summary>
  public required string VariableName { get; set; }

  /// <summary>
  /// Value to use.
  /// </summary>
  public required object? Value { get; set; }
}

/// <summary>
/// Run a user defined script.
/// </summary>
[CustomBlock(
  "create_script_parameter",
  "Scripts",
  @"{
      ""message0"": ""ScriptParameter %1 %2 %3 %4 %5"",
      ""args0"": [
          {
            ""type"": ""input_dummy""
          },
          {
              ""type"": ""field_label_serializable"",
              ""name"": ""NAME"",
              ""text"": ""Variable name""
          },
          {
            ""type"": ""input_value"",
            ""name"": ""NAME"",
            ""check"": ""String""
          },
          {
              ""type"": ""field_label_serializable"",
              ""name"": ""VALUE"",
              ""text"": ""Value""
          },
          {
            ""type"": ""input_value"",
            ""name"": ""VALUE""
          }
      ],
      ""output"": ""run_script_parameter"",
      ""colour"": 230,
      ""tooltip"": ""Create a script parameter"",
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
public class CreateRunScriptParameter : Block
{
  /// <inheritdoc/>
  public override async Task<object?> EvaluateAsync(Context context)
  {
    return new RunScriptParameter
    {
      VariableName = await Values.EvaluateAsync<string>("NAME", context),
      Value = await Values.EvaluateAsync("VALUE", context)
    };
  }
}