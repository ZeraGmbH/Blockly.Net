using System.Xml;

namespace BlocklyNet.Extensions.Models.Xml;

/// <summary>
/// Represents aa loaded XML file.
/// </summary>
public class XmlFile()
{
    /// <summary>
    /// The XML object representation.
    /// </summary>
    private readonly XmlDocument _Document = new();

    /// <summary>
    /// Parse content string to an XML document.
    /// </summary>
    /// <param name="content">XML string representation.</param>
    public XmlFile(string content) : this()
    {
        // Just parse the string representation and create the corresponding XML DOM.
        _Document.LoadXml(content);
    }

    /// <summary>
    /// Lookup nodes.
    /// </summary>
    /// <param name="xpath">XPath query string.</param>
    /// <returns>List of nodes found.</returns>
    public List<XmlNode> Query(string xpath)
    {
        var nodes = new List<XmlNode>();
        var list = _Document.SelectNodes(xpath);

        if (list != null)
            for (var i = 0; i < list.Count; i++)
            {
                var node = list[i];

                if (node != null)
                    nodes.Add(new(node));
            }

        return nodes;
    }
}