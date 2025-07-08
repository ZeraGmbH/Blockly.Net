using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;
using BlocklyNet.Extensions.Models.Xml;

namespace BlocklyNet.Extensions;


/// <summary>
/// Add Xml node/file to parent
/// </summary>
[CustomBlock(
    "xml_add_to_parent",
    "XML",
    @"{
        ""message0"": ""AddToXmlParent %1 Xml Parent %2 Content %3"",
        ""args0"": [
            {
                ""type"": ""input_dummy""
            },
            {
                ""type"": ""input_value"",
                ""name"": ""PARENT"",
                ""check"": [""xml_file"", ""xml_node""]
            },
            {
                ""type"": ""input_value"",
                ""name"": ""CONTENT"",
                ""check"": [""xml_node"", ""String""]
            }
        ],
        ""previousStatement"": null,
        ""nextStatement"": null,
        ""colour"": ""#107159"",
        ""tooltip"": ""Append an XML Node or File to a parent"",
        ""helpUrl"": """"
    }",
    ""
)]
public class AddToXmlParent : Block
{
    /// <inheritdoc/>
    protected override async Task<object?> EvaluateAsync(Context context)
    {
        var parent = await Values.EvaluateAsync<XmlNodeOrFile>("PARENT", context);
        var content = await Values.EvaluateAsync("CONTENT", context);

        if (content is string contentString)
        {
            parent.AddStringToXml(contentString);
        }
        else if (content is XmlNode contentNode)
        {
            contentNode.AddToParent(parent);
        }
        else
            throw new ArgumentException("Unrecognized input type");

        return await base.EvaluateAsync(context);
    }
}