using NUnit.Framework;

namespace BlocklyNetTests.Core;

[TestFixture]
public class LogicTests : TestEnvironment
{
    [Test]
    public async Task Can_Report_True_Async()
    {
        var script = Engine.Parser.Parse(@"
            <xml>
                <block type=""logic_boolean"">
                    <field name=""BOOL"">TRUE</field>
                </block>
            </xml>");

        Assert.That(await script.RunAsync(Site.Object), Is.True);
    }

    [Test]
    public async Task Can_Or_Async()
    {
        var script = Engine.Parser.Parse(@"
            <xml xmlns=""http://www.w3.org/1999/xhtml"">
            <block type=""logic_operation"">
                <field name=""OP"">OR</field>
                <value name=""A"">
                <block type=""logic_boolean"">
                    <field name=""BOOL"">FALSE</field>
                </block>
                </value>
                <value name=""B"">
                <block type=""logic_boolean"">
                    <field name=""BOOL"">TRUE</field>
                </block>
                </value>
            </block>
            </xml>");

        Assert.That(await script.RunAsync(Site.Object), Is.True);
    }

    [Test]
    public async Task Can_And_Async()
    {
        var script = Engine.Parser.Parse(@"
            <xml xmlns=""http://www.w3.org/1999/xhtml"">
            <block type=""logic_operation"">
                <field name=""OP"">AND</field>
                <value name=""A"">
                <block type=""logic_boolean"">
                    <field name=""BOOL"">FALSE</field>
                </block>
                </value>
                <value name=""B"">
                <block type=""logic_boolean"">
                    <field name=""BOOL"">TRUE</field>
                </block>
                </value>
            </block>
            </xml>");

        Assert.That(await script.RunAsync(Site.Object), Is.False);
    }

    [Test]
    public async Task Can_Not_Async()
    {
        var script = Engine.Parser.Parse(@"
            <xml>
            <block type=""logic_negate"">
                <value name=""BOOL"">
                <block type=""logic_boolean"">
                    <field name=""BOOL"">TRUE</field>
                </block>
                </value>
            </block>
            </xml>");

        Assert.That(await script.RunAsync(Site.Object), Is.False);
    }

    [Test]
    public async Task Can_Report_Null_Async()
    {
        var script = Engine.Parser.Parse(@"
            <xml>
            <block type=""logic_null""></block>
            </xml>");

        Assert.That(await script.RunAsync(Site.Object), Is.Null);
    }

    [Test]
    public async Task Can_Switch_Ternary_Async()
    {
        var script = Engine.Parser.Parse(@"
            <xml>
            <block type=""logic_ternary"">
                <value name=""IF"">
                <block type=""logic_boolean"">
                    <field name=""BOOL"">TRUE</field>
                </block>
                </value>
                <value name=""THEN"">
                <block type=""logic_boolean"">
                    <field name=""BOOL"">FALSE</field>
                </block>
                </value>
                <value name=""ELSE"">
                <block type=""logic_boolean"">
                    <field name=""BOOL"">TRUE</field>
                </block>
                </value>
            </block>
            </xml>");

        Assert.That(await script.RunAsync(Site.Object), Is.False);
    }
}