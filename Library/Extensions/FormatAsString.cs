using System.Collections;
using System.Reflection;
using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;

namespace BlocklyNet.Extensions;

/// <summary>
/// Format anything as string.
/// </summary>
[CustomBlock(
    "format_as_string",
    "",
    @"{
        ""message0"": ""FormatAsString %1 Value %2 Format %3"",
        ""args0"": [
            {
                ""type"": ""input_dummy""
            },
            {
                ""type"": ""input_value"",
                ""name"": ""VALUE""
            },
            {
                ""type"": ""input_value"",
                ""name"": ""FORMAT"",
                ""check"": ""String""
            }
        ],
        ""output"": ""String"",
        ""colour"": ""#107159"",
        ""tooltip"": ""Create a string from anything."",
        ""helpUrl"": """"
    }",
    @"{
        ""inputs"": {
            ""FORMAT"": {
                ""shadow"": {
                    ""type"": ""text"",
                    ""fields"": {
                        ""TEXT"": ""G""
                    }
                }
            }
        }
    }"
)]
public class FormatAsString : Block
{
    /// <inheritdoc/>
    protected override async Task<object?> EvaluateAsync(Context context)
    {
        var value = await Values.EvaluateAsync("VALUE", context);
        var format = await Values.EvaluateAsync<string>("FORMAT", context);

        if (value is not IEnumerable list || value.GetType() == typeof(string))
            return string.Format($"{{0:{format}}}", value);

        var all = new List<string>();

        foreach (var v in list)
            all.Add(string.Format($"{{0:{format}}}", v));

        return $"[{string.Join(", ", all)}]";
    }
}
