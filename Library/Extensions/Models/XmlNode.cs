using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace BlocklyNet.Extensions.Models.Xml;

/// <summary>
/// 
/// </summary>
public abstract class XmlNodeOrFile
{
    /// <summary>
    /// Add an XML string to the XML
    /// </summary>
    /// <param name="content">The content to add.</param>
    public abstract void AddStringToXml(string content);

}

/// <summary>
/// A single node from a query against an XML DOM.
/// </summary>
public class XmlNode() : XmlNodeOrFile
{
    /// <summary>
    /// Attached node.
    /// </summary>
    internal System.Xml.XmlNode _Node = null!;

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

    /// <inheritdoc/>
    public override void AddStringToXml(string content)
    {
        _Node.InnerXml += content;
    }

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

    internal void AddToParent(XmlNodeOrFile parent)
    {
        if (_Node != null) throw new InvalidOperationException("Node is already part of an XML tree");

        XmlElement element = null!;

        if (parent is XmlFile file)
        {
            if (file._Document.DocumentElement != null)
                throw new InvalidOperationException("Document has already a root element.");

            element = file._Document.CreateElement(Name, Namespace);

            _Node = element;
            file._Document.AppendChild(_Node);
        }
        else if (parent is XmlNode node)
        {
            if (node._Node.OwnerDocument == null)
                throw new InvalidOperationException("Parent Node is not Part of an XML tree yet.");

            element = node._Node.OwnerDocument!.CreateElement(Name, Namespace);

            _Node = element;
            node._Node.AppendChild(element);
        }

        foreach (var attr in Attributes)
        {
            element.SetAttribute(attr.Name, attr.Namespace, attr.Value);
        }
        element.InnerText = Content;
    }
}