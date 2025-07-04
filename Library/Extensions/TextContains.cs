using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;

namespace BlocklyNet.Extensions;


/// <summary>
/// Block to check if a text contains, startsWith or endsWith a substring
/// </summary>
[CustomBlock(
    "text_contains_starts_ends",
    "",
    @"{
        ""message0"": ""text %1 %2 %3 case %4"",
        ""args0"": [
            {
                ""type"": ""input_value"",
                ""name"": ""VALUE"",
                ""check"": ""String""
            },
            {
                ""type"": ""field_dropdown"",
                ""name"": ""METHOD"",
                ""options"": [
                [""contains"", ""CONTAINS""],
                [""startsWith"", ""STARTSWITH""],
                [""endsWith"", ""ENDSWITH""]
                ]
            },
            {
                ""type"": ""input_value"",
                ""name"": ""SEARCH"",
                ""check"": ""String""
            },
            {
                ""type"": ""field_dropdown"",
                ""name"": ""CASESENSITIVE"",
                ""options"": [
                [""insensitive"", ""FALSE""],
                [""sensitive"", ""TRUE""]
                ]
            }
        ],
        ""inputsInline"": ""true"",
        ""output"": ""Boolean"",
        ""colour"": ""165"",
        ""tooltip"": ""Create a string from anything."",
        ""helpUrl"": """"
    }",
    @""
    )]
public class TextContains : Block
{
    /// <inheritdoc/>
    protected override async Task<object?> EvaluateAsync(Context context)
    {
        var value = await Values.EvaluateAsync<string>("VALUE", context);
        var searchString = await Values.EvaluateAsync<string>("SEARCH", context);
        StringComparison stringComparison = Fields["CASESENSITIVE"] == "TRUE" ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase;

        return Fields["METHOD"] switch
        {
            "CONTAINS" => value.Contains(searchString, stringComparison),
            "STARTSWITH" => value.StartsWith(searchString, stringComparison),
            "ENDSWITH" => value.EndsWith(searchString, stringComparison),
            _ => (object)false,
        };
    }
}