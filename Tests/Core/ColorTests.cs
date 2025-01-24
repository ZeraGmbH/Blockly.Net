using NUnit.Framework;

namespace BlocklyNetTests.Core;

[TestFixture]
public class ColorTests : TestEnvironment
{
    [Test]
    public async Task Can_Parse_Color_Async()
    {
        var script = Engine.Parser.Parse(@"
            <xml>
            <block type=""colour_picker"">
                <field name=""COLOUR"">#ff0000</field>
            </block>
            </xml>");

        Assert.That(await script.RunAsync(Site.Object), Is.EqualTo("#ff0000"));
    }

    [Test]
    public async Task Can_Get_Random_Color_Async()
    {
        var script = Engine.Parser.Parse(@"
            <xml>
            <block type=""colour_random""></block>
            </xml>");

        var output = (string)(await script.RunAsync(Site.Object))!;

        Assert.That(output, Has.Length.EqualTo(7));
        Assert.That(output[0], Is.EqualTo('#'));

        Assert.That(await script.RunAsync(Site.Object), Is.Not.EqualTo(output));
    }

    [Test]
    public async Task Can_Create_From_RGB_Async()
    {
        var script = Engine.Parser.Parse(@"
            <xml>
            <block type=""colour_rgb"" id=""JM:^k(U=gB8g^%oXS3}#"" x=""188"" y=""88"">
                <value name=""RED"">
                <shadow type=""math_number"" id="":R|JG4bNKO%{Imqb80Ra"">
                    <field name=""NUM"">255</field>
                </shadow>
                </value>
                <value name=""GREEN"">
                <shadow type=""math_number"" id=""u)A6y/5^OS?,@4_[qH#f"">
                    <field name=""NUM"">0</field>
                </shadow>
                </value>
                <value name=""BLUE"">
                <shadow type=""math_number"" id=""QygYvsLu_]am-bn9M_S-"">
                    <field name=""NUM"">1</field>
                </shadow>
                </value>
            </block>
            </xml>");

        Assert.That(await script.RunAsync(Site.Object), Is.EqualTo("#ff0001"));
    }

    [Test]
    public async Task Can_Blend_Async()
    {
        var script = Engine.Parser.Parse(@"
            <xml>
            <block type=""colour_blend"">
                <value name=""COLOUR1"">
                <shadow type=""colour_picker"">
                    <field name=""COLOUR"">#ff0000</field>
                </shadow>
                </value>
                <value name=""COLOUR2"">
                <shadow type=""colour_picker"">
                    <field name=""COLOUR"">#3333ff</field>
                </shadow>
                </value>
                <value name=""RATIO"">
                <shadow type=""math_number"">
                    <field name=""NUM"">0.2</field>
                </shadow>
                </value>
            </block>
            </xml>");

        Assert.That(await script.RunAsync(Site.Object), Is.EqualTo("#d60a33"));
    }
}