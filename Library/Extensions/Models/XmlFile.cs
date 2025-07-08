using Xml = System.Xml;

namespace BlocklyNet.Extensions.Models;

/// <summary>
/// Represents aa loaded XML file.
/// </summary>
public class XmlFile() : XmlNodeOrFile
{
    /// <inheritdoc/>
    internal protected override Xml.XmlDocument Document { get; } = new();

    /// <inheritdoc/>
    internal protected override Xml.XmlNode Node => Document;

    /// <summary>
    /// Parse content string to an XML document.
    /// </summary>
    /// <param name="content">XML string representation.</param>
    public XmlFile(string content) : this()
    {
        // Just parse the string representation and create the corresponding XML DOM.
        Document.LoadXml(content);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        using var writer = new StringWriter();

        Document.Save(writer);

        return writer.ToString();
    }
}