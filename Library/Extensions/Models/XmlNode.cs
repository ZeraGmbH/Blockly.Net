using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlocklyNet.Extensions.Models.Xml;

/// <summary>
/// A single node from a query against an XML DOM.
/// </summary>
public class XmlNode()
{
    /// <summary>
    /// Create a new node representation.
    /// </summary>
    /// <param name="node">The underlying node.</param>
    public XmlNode(System.Xml.XmlNode node) : this()
    {
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
}