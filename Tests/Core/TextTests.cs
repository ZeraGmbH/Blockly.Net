using NUnit.Framework;

namespace BlocklyNetTests.Core;

[TestFixture]
public class TextTests : TestEnvironment
{
    [Test]
    public async Task Can_Get_Length()
    {
        var script = Engine.Parser.Parse(@"
            <xml>
            <block type=""text_length"">
                <value name=""VALUE"">
                <shadow type=""text"">
                    <field name=""TEXT"">abc</field>
                </shadow>
                </value>
            </block>
            </xml>");

        Assert.That(await script.Run(Site.Object), Is.EqualTo(3));
    }

    [Test]
    public async Task Can_Test_On_Empty()
    {
        var script = Engine.Parser.Parse(@"
            <xml>
            <block type=""text_isEmpty"">
                <value name=""VALUE"">
                <shadow type=""text"">
                    <field name=""TEXT""></field>
                </shadow>
                </value>
            </block>
            </xml>");

        Assert.That(await script.Run(Site.Object), Is.True);
    }

    [Test]
    public async Task Can_Trim()
    {
        var script = Engine.Parser.Parse(@"
            <xml>
            <block type=""text_trim"">
                <field name=""MODE"">BOTH</field>
                <value name=""TEXT"">
                <shadow type=""text"">
                    <field name=""TEXT""> ab c </field>
                </shadow>
                </value>
            </block>
            </xml>");

        Assert.That(await script.Run(Site.Object), Is.EqualTo("ab c"));
    }

    [Test]
    public async Task Can_Convert_To_Title_Case()
    {
        var script = Engine.Parser.Parse(@"
            <xml>
            <block type=""text_changeCase"">
                <field name=""CASE"">TITLECASE</field>
                <value name=""TEXT"">
                <shadow type=""text"">
                    <field name=""TEXT"">hello world</field>
                </shadow>
                </value>
            </block>
            </xml>");

        Assert.That(await script.Run(Site.Object), Is.EqualTo("Hello World"));
    }

    [Test]
    public async Task Can_Append()
    {
        var script = Engine.Parser.Parse(@"
        <xml>
        <variables>
            <variable type="""">x</variable>
        </variables>
        <block type=""variables_set"">
            <field name=""VAR"">x</field>
            <value name=""VALUE"">
            <block type=""text"">
                <field name=""TEXT"">foo</field>
            </block>
            </value>
            <next>
                <block type=""text_append"">
                    <field name=""VAR"">x</field>
                    <value name=""TEXT"">
                        <shadow type=""text"">
                            <field name=""TEXT"">bar</field>
                        </shadow>
                    </value>
                    <next>
                        <block type=""variables_get"">
                            <field name=""VAR"">x</field>
                        </block>
                    </next>
                </block>
            </next>
        </block>
        </xml>");

        Assert.That(await script.Run(Site.Object), Is.EqualTo("foobar"));
    }

    [Test]
    public async Task Can_Join()
    {
        var script = Engine.Parser.Parse(@"
        <xml xmlns=""http://www.w3.org/1999/xhtml"">
        <variables>
            <variable>x</variable>
        </variables>
        <block type=""variables_set"">
            <field name=""VAR"">x</field>
            <value name=""VALUE"">
            <block type=""text_join"">
                <mutation items=""3""></mutation>
                <value name=""ADD0"">
                <block type=""text"">
                    <field name=""TEXT"">foo</field>
                </block>
                </value>
                <value name=""ADD1"">
                <block type=""text"">
                    <field name=""TEXT"">bar</field>
                </block>
                </value>
            </block>
            </value>
            <next>
                <block type=""variables_get"">
                    <field name=""VAR"">x</field>
                </block>
            </next>
        </block>
        </xml>");

        Assert.That(await script.Run(Site.Object), Is.EqualTo("foobar"));
    }

    [Test]
    public async Task Can_Find_Index()
    {
        var script = Engine.Parser.Parse(@"
            <xml>
            <block type=""text_indexOf"">
                <field name=""END"">FIRST</field>
                <value name=""VALUE"">
                <block type=""text"">
                    <field name=""TEXT"">foo bar baz</field>
                </block>
                </value>
                <value name=""FIND"">
                <shadow type=""text"">
                    <field name=""TEXT"">bar</field>
                </shadow>
                </value>
            </block>
            </xml>");

        Assert.That(await script.Run(Site.Object), Is.EqualTo(5));
    }
}