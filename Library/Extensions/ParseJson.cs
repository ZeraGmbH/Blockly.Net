using System.Dynamic;
using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;
using Newtonsoft.Json;

namespace BlocklyNet.Extensions;

/// <summary>
/// Set a progress for the current script execution.
/// </summary>
[CustomBlock(
    "parse_json",
    "",
    @"{
        ""message0"": ""ParseJSON %1 %2 %3"",
        ""args0"": [
            {
                ""type"": ""input_dummy""
            },
            {
                ""type"": ""field_label_serializable"",
                ""name"": ""JSON"",
                ""text"": ""JSON string""
            },
            {
                ""type"": ""input_value"",
                ""name"": ""JSON"",
                ""check"": ""String""
            }
        ],
        ""output"": null,
        ""colour"": ""#107159"",
        ""tooltip"": ""Create object from JSON string"",
        ""helpUrl"": """"
    }",
    @"{
        ""inputs"": {
            ""JSON"": {
                ""shadow"": {
                    ""type"": ""text"",
                    ""fields"": {
                        ""TEXT"": ""{}""
                    }
                }
            }
        }
    }"
)]
public class ParseJson : Block
{
    /// <inheritdoc/>
    public override async Task<object?> Evaluate(Context context)
    {
        /* Currently (.NET 8) rely on Newtonsoft.JSON since System.Text.Json will not handle ExpandoObject correctly: fields values will be JsonElement. */
        return JsonConvert.DeserializeObject<ExpandoObject>(await Values.Evaluate<string>("JSON", context));
    }
}
