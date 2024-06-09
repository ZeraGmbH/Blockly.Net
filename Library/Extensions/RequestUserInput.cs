using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;

namespace BlocklyNet.Extensions;


/// <summary>
/// Request data from the user.
/// </summary>
[CustomBlock(
  "request_user_input",
  "Scripts",
  @"{
      ""message0"": ""RequestUserInput %1 %2 %3 %4 %5"",
      ""args0"": [
          {
            ""type"": ""input_dummy""
          },
          {
              ""type"": ""field_label_serializable"",
              ""name"": ""KEY"",
              ""text"": ""Key""
          },
          {
            ""type"": ""input_value"",
            ""name"": ""KEY"",
            ""check"": ""String""
          },
          {
              ""type"": ""field_label_serializable"",
              ""name"": ""TYPE"",
              ""text"": ""Type""
          },
          {
            ""type"": ""input_value"",
            ""name"": ""TYPE""
          }
      ],
      ""output"": null,
      ""colour"": 230,
      ""tooltip"": ""Request input from the user"",
      ""helpUrl"": """"
  }",
  @"{
    ""inputs"": {
      ""KEY"": {
        ""shadow"": {
          ""type"": ""text"",
          ""fields"": {
            ""TEXT"": """"
          }
        }
      },
      ""TYPE"": {
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
public class RequestUserInput : Block
{
  /// <inheritdoc/>
  public override async Task<object?> Evaluate(Context context)
  {
    return await context.Engine.GetUserInput<object>(
      await Values.Evaluate<string>("KEY", context),
      await Values.Evaluate<string>("TYPE", context, false));
  }
}