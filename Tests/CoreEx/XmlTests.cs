using BlocklyNet.Core.Model;
using BlocklyNet.Extensions;
using BlocklyNet.Extensions.Models.Xml;
using NUnit.Framework;

namespace BlocklyNetTests.CoreEx;

[TestFixture]
public class XmlTests : TestEnvironment
{
    private const string Sample1 = @"<?xml version=""1.0"" encoding=""UTF-8""?>
        <CATALOG>
        <CD mode=""oldie"">
            <TITLE>Empire Burlesque</TITLE>
            <ARTIST>Bob Dylan</ARTIST>
            <COUNTRY>USA</COUNTRY>
            <COMPANY>Columbia</COMPANY>
            <PRICE>10.90</PRICE>
            <YEAR>1985</YEAR>
        </CD>
        <CD>
            <TITLE>Hide your heart</TITLE>
            <ARTIST>Bonnie Tyler</ARTIST>
            <COUNTRY>UK</COUNTRY>
            <COMPANY>CBS Records</COMPANY>
            <PRICE>9.90</PRICE>
            <YEAR>1988</YEAR>
        </CD>
        <CD>
            <TITLE>Greatest Hits</TITLE>
            <ARTIST>Dolly Parton</ARTIST>
            <COUNTRY>USA</COUNTRY>
            <COMPANY>RCA</COMPANY>
            <PRICE>9.90</PRICE>
            <YEAR>1982</YEAR>
        </CD>
        </CATALOG>";

    [Test]
    public async Task Can_Parse_Xml_Async()
    {
        /* Build block tree. */
        var block = new CreateXmlDocument
        {
            Values = { new() { Name = "CONTENT", Block = CreateStringBlock(Sample1) } }
        };

        /* Parse XML. */
        var file = await block.EvaluateAsync(new Context(Site.Object));

        /* Validate result. */
        Assert.That(file, Is.TypeOf<XmlFile>());
    }
}