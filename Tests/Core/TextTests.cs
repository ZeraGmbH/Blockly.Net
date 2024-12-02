using NUnit.Framework;

namespace BlocklyNetTests.Core;

[TestFixture]
public class TextTests : TestEnvironment
{
    [Test]
    public async Task Can_Get_Length_Async()
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

        Assert.That(await script.RunAsync(Site.Object), Is.EqualTo(3));
    }

    [Test]
    public async Task Can_Test_On_Empty_Async()
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

        Assert.That(await script.RunAsync(Site.Object), Is.True);
    }

    [Test]
    public async Task Can_Trim_Async()
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

        Assert.That(await script.RunAsync(Site.Object), Is.EqualTo("ab c"));
    }

    [Test]
    public async Task Can_Convert_To_Title_Case_Async()
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

        Assert.That(await script.RunAsync(Site.Object), Is.EqualTo("Hello World"));
    }

    [Test]
    public async Task Can_Append_Async()
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

        Assert.That(await script.RunAsync(Site.Object), Is.EqualTo("foobar"));
    }

    [Test]
    public async Task Can_Join_Async()
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

        Assert.That(await script.RunAsync(Site.Object), Is.EqualTo("foobar"));
    }

    [Test]
    public async Task Can_Find_Index_Async()
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

        Assert.That(await script.RunAsync(Site.Object), Is.EqualTo(5));
    }

    [TestCase("FIRST", 0, "LAST", 0, "Bla01234Blo")]
    [TestCase("FROM_START", 3, "LAST", 0, "a01234Blo")]
    [TestCase("FIRST", 0, "FROM_END", 1, "Bla01234Blo")]
    [TestCase("FIRST", 0, "FROM_END", 2, "Bla01234Bl")]
    [TestCase("FROM_START", 4, "FROM_END", 2, "01234Bl")]
    public async Task Can_Extract_Part_Of_String_Async(string where1, int at1, string where2, int at2, string expected)
    {
        var template = @"
            <xml>
                <block type=""text_getSubstring"">
                    <field name=""WHERE1"">$$WHERE1$$</field>
                    <field name=""WHERE2"">$$WHERE2$$</field>
                    <value name=""AT1"">
                        <block type=""math_number"">
                            <field name=""NUM"">$$AT1$$</field>
                        </block>
                    </value>
                    <value name=""AT2"">
                        <block type=""math_number"">
                            <field name=""NUM"">$$AT2$$</field>
                        </block>
                    </value>
                    <value name=""STRING"">
                        <block type=""text"">
                            <field name=""TEXT"">Bla01234Blo</field>
                        </block>
                    </value>
                </block>
            </xml>";

        var script = Engine.Parser.Parse(
            template
                .Replace("$$WHERE1$$", where1)
                .Replace("$$WHERE2$$", where2)
                .Replace("$$AT1$$", at1.ToString())
                .Replace("$$AT2$$", at2.ToString())
        );

        Assert.That(await script.RunAsync(Site.Object), Is.EqualTo(expected));
    }
}