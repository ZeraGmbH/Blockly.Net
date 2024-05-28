using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;

namespace BlocklyNet.Extensions;

/// <summary>
/// Set a progress for the current script execution.
/// </summary>
[CustomBlock(
    "throw_exception",
    "",
    @"{
        ""message0"": ""Throw %1 %2 %3"",
        ""args0"": [
            {
            ""type"": ""input_dummy""
            },
             {
                ""type"": ""field_label_serializable"",
                ""name"": ""MESSAGE"",
                ""text"": ""Message""
            },
            {
                ""type"": ""input_value"",
                ""name"": ""MESSAGE"",
                ""check"": ""String""
            }
        ],
        ""previousStatement"": null,
        ""colour"": ""#107159"",
        ""tooltip"": ""Throw an exception"",
        ""helpUrl"": """"
    }",
    @"{
        ""inputs"": {
            ""MESSAGE"": {
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
public class Throw : Block
{
    /// <inheritdoc/>
    public override async Task<object?> Evaluate(Context context)
    {
        throw new Exception(await Values.Evaluate<string>("MESSAGE", context));
    }
}
