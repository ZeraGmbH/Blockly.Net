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
                ""name"": ""SOURCE"",
                ""text"": ""XML File""
            },
            {
                ""type"": ""input_value"",
                ""name"": ""SOURCE"",
                ""check"": [""xml_file"", ""xml_node""]
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
        var source = await Values.EvaluateAsync("SOURCE", context);
        var query = await Values.EvaluateAsync<string>("XPATH", context);

        if (source is XmlFile document)
            return document.Query(query);

        if (source is XmlNode node)
            return node.Query(query);

        throw new ArgumentException("not an XML node", "SOURCE");
    }
}
