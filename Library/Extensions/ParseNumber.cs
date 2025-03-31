using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;

namespace BlocklyNet.Extensions;

/// <summary>
/// Set a progress for the current script execution.
/// </summary>
[CustomBlock(
    "parse_number",
    "",
    @"{
        ""message0"": ""ParseNumber %1 %2 %3"",
        ""args0"": [
            {
                ""type"": ""input_dummy""
            },
            {
                ""type"": ""field_label_serializable"",
                ""name"": ""NUMBER"",
                ""text"": ""Number as string""
            },
            {
                ""type"": ""input_value"",
                ""name"": ""NUMBER"",
                ""check"": ""String""
            }
        ],
        ""output"": null,
        ""colour"": ""#107159"",
        ""tooltip"": ""Create number from string"",
        ""helpUrl"": """"
    }",
    ""
)]
public class ParseNumber : Block
{
    /// <inheritdoc/>
    protected override async Task<object?> EvaluateAsync(Context context)
        => double.Parse(await Values.EvaluateAsync<string>("NUMBER", context));
}
