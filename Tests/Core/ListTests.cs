using NUnit.Framework;

namespace BlocklyNetTests.Core;

[TestFixture]
public class ListTests : TestEnvironment
{
    [Test]
    public async Task Can_Create_List_Async()
    {
        var script = Engine.Parser.Parse(@"
            <xml xmlns=""http://www.w3.org/1999/xhtml"">
            <block type=""lists_create_with"">
                <mutation items=""3""></mutation>
                <value name=""ADD0"">
                <block type=""text"">
                    <field name=""TEXT"">x</field>
                </block>
                </value>
                <value name=""ADD1"">
                <block type=""text"">
                    <field name=""TEXT"">y</field>
                </block>
                </value>
                <value name=""ADD2"">
                <block type=""text"">
                    <field name=""TEXT"">z</field>
                </block>
                </value>
            </block>
            </xml>");

        Assert.That(await script.RunAsync(Site.Object), Is.EqualTo(new string[] { "x", "y", "z" }));
    }

    [Test]
    public async Task Can_Split_List_Async()
    {
        var script = Engine.Parser.Parse(@"
            <xml xmlns=""http://www.w3.org/1999/xhtml"">
            <block type=""lists_split"">
                <mutation mode=""SPLIT""></mutation>
                <field name=""MODE"">SPLIT</field>
                <value name=""INPUT"">
                <block type=""text"">
                    <field name=""TEXT"">x,y,z</field>
                </block>
                </value>
                <value name=""DELIM"">
                <shadow type=""text"">
                    <field name=""TEXT"">,</field>
                </shadow>
                </value>
            </block>
            </xml>");

        Assert.That(await script.RunAsync(Site.Object), Is.EqualTo(new string[] { "x", "y", "z" }));
    }

    [Test]
    public async Task Can_Join_Lists_Async()
    {
        var script = Engine.Parser.Parse(@"
            <xml xmlns=""http://www.w3.org/1999/xhtml"">
            <block type=""lists_split"">
                <mutation mode=""JOIN""></mutation>
                <field name=""MODE"">JOIN</field>
                <value name=""INPUT"">
                <block type=""lists_create_with"">
                    <mutation items=""3""></mutation>
                    <value name=""ADD0"">
                    <block type=""text"">
                        <field name=""TEXT"">x</field>
                    </block>
                    </value>
                    <value name=""ADD1"">
                    <block type=""text"">
                        <field name=""TEXT"">y</field>
                    </block>
                    </value>
                    <value name=""ADD2"">
                    <block type=""text"">
                        <field name=""TEXT"">z</field>
                    </block>
                    </value>
                </block>
                </value>
                <value name=""DELIM"">
                <shadow type=""text"">
                    <field name=""TEXT"">,</field>
                </shadow>
                </value>
            </block>
            </xml>");

        Assert.That(await script.RunAsync(Site.Object), Is.EqualTo("x,y,z"));
    }

    [Test]
    public async Task Can_Get_Length_Async()
    {
        var script = Engine.Parser.Parse(@"
            <xml xmlns=""http://www.w3.org/1999/xhtml"">
            <block type=""lists_length"">
                <value name=""VALUE"">
                <block type=""lists_split"">
                    <mutation mode=""SPLIT""></mutation>
                    <field name=""MODE"">SPLIT</field>
                    <value name=""INPUT"">
                    <block type=""text"">
                        <field name=""TEXT"">a,b,c</field>
                    </block>
                    </value>
                    <value name=""DELIM"">
                    <shadow type=""text"">
                        <field name=""TEXT"">,</field>
                    </shadow>
                    </value>
                </block>
                </value>
            </block>
            </xml>");

        Assert.That(await script.RunAsync(Site.Object), Is.EqualTo(3));
    }

    [Test]
    public async Task Can_Repeat_Async()
    {
        var script = Engine.Parser.Parse(@"
            <xml xmlns=""http://www.w3.org/1999/xhtml"">
            <block type=""lists_repeat"">
                <value name=""ITEM"">
                <block type=""text"">
                    <field name=""TEXT"">hello</field>
                </block>
                </value>
                <value name=""NUM"">
                <shadow type=""math_number"">
                    <field name=""NUM"">3</field>
                </shadow>
                </value>
            </block>
            </xml>");

        Assert.That(await script.RunAsync(Site.Object), Is.EqualTo(new string[] { "hello", "hello", "hello" }));
    }

    [Test]
    public async Task Can_Test_For_Empty_Async()
    {
        var script = Engine.Parser.Parse(@"
            <xml xmlns=""http://www.w3.org/1999/xhtml"">
            <block type=""lists_isEmpty"">
                <value name=""VALUE"">
                <block type=""lists_create_with"">
                    <mutation items=""0""></mutation>
                </block>
                </value>
            </block>
            </xml>");

        Assert.That(await script.RunAsync(Site.Object), Is.True);
    }

    [Test]
    public async Task Can_Find_Index_Async()
    {
        var script = Engine.Parser.Parse(@"
            <xml xmlns=""http://www.w3.org/1999/xhtml"">
            <variables></variables>
            <block type=""lists_indexOf"">
                <field name=""END"">FIRST</field>
                <value name=""VALUE"">
                <block type=""lists_split"">
                    <mutation mode=""SPLIT""></mutation>
                    <field name=""MODE"">SPLIT</field>
                    <value name=""INPUT"">
                    <block type=""text"">
                        <field name=""TEXT"">foo,bar,baz</field>
                    </block>
                    </value>
                    <value name=""DELIM"">
                    <shadow type=""text"">
                        <field name=""TEXT"">,</field>
                    </shadow>
                    </value>
                </block>
                </value>
                <value name=""FIND"">
                <block type=""text"">
                    <field name=""TEXT"">bar</field>
                </block>
                </value>
            </block>
            </xml>");

        Assert.That(await script.RunAsync(Site.Object), Is.EqualTo(2));
    }

    [Test]
    public async Task Can_Get_Element_Async()
    {
        var script = Engine.Parser.Parse(@"
            <xml xmlns=""http://www.w3.org/1999/xhtml"">
            <block type=""lists_getIndex"">
                <mutation statement=""false"" at=""true""></mutation>
                <field name=""MODE"">GET</field>
                <field name=""WHERE"">FROM_START</field>
                <value name=""VALUE"">
                <block type=""lists_split"">
                    <mutation mode=""SPLIT""></mutation>
                    <field name=""MODE"">SPLIT</field>
                    <value name=""INPUT"">
                    <block type=""text"">
                        <field name=""TEXT"">foo,bar,baz</field>
                    </block>
                    </value>
                    <value name=""DELIM"">
                    <shadow type=""text"">
                        <field name=""TEXT"">,</field>
                    </shadow>
                    </value>
                </block>
                </value>
                <value name=""AT"">
                <block type=""math_number"">
                    <field name=""NUM"">2</field>
                </block>
                </value>
            </block>
            </xml>");

        Assert.That(await script.RunAsync(Site.Object), Is.EqualTo("bar"));
    }
}