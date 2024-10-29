using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;
using BlocklyNet.Extensions.Models.Xml;

namespace BlocklyNet.Extensions;

/// <summary>
/// Query an XML document.
/// </summary>
[CustomBlock(
    "xml_query_document",
    "XML",
    @"{
        ""message0"": ""QueryXmlDocument %1 %2 %3 %4 %5"",
        ""args0"": [
            {
                ""type"": ""input_dummy""
            },
            {
                ""type"": ""field_label_serializable"",
                ""name"": ""FILE"",
                ""text"": ""XML File""
            },
            {
                ""type"": ""input_value"",
                ""name"": ""FILE"",
                ""check"": ""xml_file""
            },
            {
                ""type"": ""field_label_serializable"",
                ""name"": ""XPATH"",
                ""text"": ""XPATH Query""
            },
            {
                ""type"": ""input_value"",
                ""name"": ""XPATH"",
                ""check"": ""String""
            }
        ],
        ""output"": [""Array"", ""Array(xml_node)""],
        ""colour"": ""#107159"",
        ""tooltip"": ""Query an XML Document."",
        ""helpUrl"": """"
    }",
    @"{
        ""inputs"": {
            ""XPATH"": {
                ""shadow"": {
                    ""type"": ""text"",
                    ""fields"": {
                        ""NUM"": """"
                    }
                }
            }
        }
    }"
)]
public class QueryXmlDocument : Block
{
    /// <inheritdoc/>
    public override async Task<object?> EvaluateAsync(Context context)
    {
        var file = await Values.EvaluateAsync<XmlFile>("FILE", context);
        var query = await Values.EvaluateAsync<string>("XPATH", context);

        return file.Query(query);
    }
}
