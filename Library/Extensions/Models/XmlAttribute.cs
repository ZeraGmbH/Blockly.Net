using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlocklyNet.Extensions.Models.Xml;

/// <summary>
/// Represents a single XML attribute on a node.
/// </summary>
public class XmlAttribute()
{
    /// <summary>
    /// Create a new attribute representation.
    /// </summary>
    /// <param name="attribute">The underlying attribute.</param>
    public XmlAttribute(System.Xml.XmlAttribute attribute) : this()
    {
        LocalName = attribute.LocalName;
        Name = attribute.Name;
        Namespace = attribute.NamespaceURI;
        Value = attribute.Value;
    }

    /// <summary>
    /// Full name of the attribute.
    /// </summary>
    [Required, NotNull]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Localname of the attribute.
    /// </summary>
    [Required, NotNull]
    public string LocalName { get; set; } = null!;

    /// <summary>
    /// Namespace of the attribute.
    /// </summary>
    [Required]
    public string Namespace { get; set; } = null!;

    /// <summary>
    /// Value of the attribute;
    /// </summary>
    [Required]
    public string Value { get; set; } = null!;
}