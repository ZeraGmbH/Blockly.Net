using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlocklyNet.Extensions.Models.Xml;

/// <summary>
/// A single node from a query against an XML DOM.
/// </summary>
public class XmlNode()
{
    /// <summary>
    /// Attached node.
    /// </summary>
    private readonly System.Xml.XmlNode _Node = null!;

    /// <summary>
    /// Create a new node representation.
    /// </summary>
    /// <param name="node">The underlying node.</param>
    public XmlNode(System.Xml.XmlNode node) : this()
    {
        _Node = node;

        Content = node.InnerText;
        LocalName = node.LocalName;
        Name = node.Name;
        Namespace = node.NamespaceURI;

        if (node.Attributes != null)
            for (var i = 0; i < node.Attributes.Count; i++)
                Attributes.Add(new(node.Attributes[i]));
    }

    /// <summary>
    /// All attributes of the node.
    /// </summary>
    [Required, NotNull]
    public List<XmlAttribute> Attributes { get; set; } = [];

    /// <summary>
    /// Full name of the node.
    /// </summary>
    [Required, NotNull]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Localname of the node.
    /// </summary>
    [Required, NotNull]
    public string LocalName { get; set; } = null!;

    /// <summary>
    /// Namespace of the node.
    /// </summary>
    [Required]
    public string Namespace { get; set; } = null!;

    /// <summary>
    /// Text content of the node;
    /// </summary>
    [Required]
    public string Content { get; set; } = null!;

    /// <summary>
    /// Lookup child nodes.
    /// </summary>
    /// <param name="xpath">XPath query string.</param>
    /// <returns>List of nodes found.</returns>
    public List<XmlNode> Query(string xpath)
    {
        var nodes = new List<XmlNode>();
        var list = _Node.SelectNodes(xpath);

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