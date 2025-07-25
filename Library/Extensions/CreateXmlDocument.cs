using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;
using BlocklyNet.Extensions.Models;

namespace BlocklyNet.Extensions;

/// <summary>
/// Create and XML document from a text context.
/// </summary>
[CustomBlock(
    "xml_create_document",
    "XML",
    @"{
        ""message0"": ""CreateXmlDocument %1 XML Content %2"",
        ""args0"": [
            {
                ""type"": ""input_dummy""
            },
            {
                ""type"": ""input_value"",
                ""name"": ""CONTENT"",
                ""check"": ""String""
            }
        ],
        ""output"": ""xml_file"",
        ""colour"": ""#107159"",
        ""tooltip"": ""Create an XML Document from a String."",
        ""helpUrl"": """"
    }",
    ""
)]
public class CreateXmlDocument : Block
{
    /// <inheritdoc/>
    protected override async Task<object?> EvaluateAsync(Context context)
    {
        var content = await Values.EvaluateAsync<string?>("CONTENT", context, false);

        return string.IsNullOrEmpty(content) ? new XmlFile() : new XmlFile(content);
    }
}
