using BlocklyNet.Core.Blocks.Math;
using BlocklyNet.Core.Blocks.Text;
using BlocklyNetTests;
using NUnit.Framework;

namespace BlocklyNetTests.Core;

[TestFixture]
public class ParserTests : TestEnvironment
{
    [Test]
    public void Can_Get_All_Blocks()
    {
        var template = @"
            <xml>
                <block type=""text_getSubstring"">
                    <field name=""WHERE1"">FIRST</field>
                    <field name=""WHERE2"">LAST</field>
                    <value name=""AT1"">
                        <block type=""math_number"">
                            <field name=""NUM"">0</field>
                        </block>
                    </value>
                    <value name=""AT2"">
                        <block type=""math_number"">
                            <field name=""NUM"">0</field>
                        </block>
                    </value>
                    <value name=""STRING"">
                        <block type=""text"">
                            <field name=""TEXT"">Nix</field>
                        </block>
                    </value>
                </block>
            </xml>";

        var blocks = Engine.Parser.Parse(template).Blocks;

        Assert.That(blocks, Has.Length.EqualTo(4));

        Assert.Multiple(() =>
        {
            Assert.That(blocks[0], Is.TypeOf<TextSubstring>());
            Assert.That(blocks[1], Is.TypeOf<MathNumber>());
            Assert.That(blocks[2], Is.TypeOf<MathNumber>());
            Assert.That(blocks[3], Is.TypeOf<TextBlock>());
        });

    }
}