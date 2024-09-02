
using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;

namespace BlocklyNet.Extensions;

/// <summary>
/// Get the current time as a string.
/// </summary>
[CustomBlock(
    "get_date_time",
    "",
    @"{
        ""message0"": ""Now %1 %2 %3"",
        ""args0"": [
            {
                ""type"": ""input_dummy""
            },
            {
                ""type"": ""field_label_serializable"",
                ""name"": ""FORMAT"",
                ""text"": ""Format""
            },
            {
                ""type"": ""input_value"",
                ""name"": ""FORMAT"",
                ""check"": ""String""
            }
        ],
        ""output"": null,
        ""colour"": ""#107159"",
        ""tooltip"": ""Get the current time"",
        ""helpUrl"": """"
    }",
    @"{
        ""inputs"": {
            ""FORMAT"": {
                ""shadow"": {
                    ""type"": ""text"",
                    ""fields"": {
                        ""TEXT"": ""dd.MM.yyyy HH:mm:ss""
                    }
                }
            }
        }
    }"
)]
public class Now : Block
{
    /// <inheritdoc/>
    public override async Task<object?> EvaluateAsync(Context context)
    {
        var format = await Values.EvaluateAsync<string>("FORMAT", context);

        return string.IsNullOrWhiteSpace(format)
            ? (double)DateTime.Now.Ticks
            : DateTime.Now.ToString(format);
    }
}
