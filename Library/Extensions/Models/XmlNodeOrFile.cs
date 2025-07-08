using Xml = System.Xml;

namespace BlocklyNet.Extensions.Models;

/// <summary>
/// 
/// </summary>
public abstract class XmlNodeOrFile
{
    /// <summary>
    /// Add an XML string to the XML
    /// </summary>
    /// <param name="content">The content to add.</param>
    public void AppendXml(string content) => Node.InnerXml += content;

    /// <summary>
    /// Get the document attach to this instance.
    /// </summary>
    internal protected abstract Xml.XmlDocument Document { get; }

    /// <summary>
    /// Node attached to this instance.
    /// </summary>
    internal protected abstract Xml.XmlNode Node { get; }

    /// <summary>
    /// Add node to the XML.
    /// </summary>
    /// <param name="node">Uninitialized node to add.</param>
    public void AppendNode(XmlNode node)
    {
        // Create the new element.
        var element = Document.CreateElement(node.Name, node.Namespace);

        // Attach all attributes.
        foreach (var attr in node.Attributes)
            element.SetAttribute(attr.Name, attr.Namespace, attr.Value);

        // Set the inner content as raw text.
        element.InnerText = node.Content;

        // Attach the element to the node.
        node.SetNode(element);

        // Add to DOM.
        Node.AppendChild(element);
    }

    /// <summary>
    /// Lookup nodes.
    /// </summary>
    /// <param name="xpath">XPath query string.</param>
    /// <returns>List of nodes found.</returns>
    public List<XmlNode> Query(string xpath)
    {
        var nodes = new List<XmlNode>();
        var list = Node.SelectNodes(xpath);

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
