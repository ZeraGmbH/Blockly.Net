using BlocklyNet.Core.Blocks.Math;
using BlocklyNet.Core.Model;
using NUnit.Framework;

namespace BlocklyNetTests.Core;

[TestFixture]
public class MathBlockTests : TestEnvironment
{
    /// <summary>
    /// See if we can run the square root block from the XML representation.
    /// </summary>
    [Test]
    public async Task Can_Get_Square_Root_From_Xml()
    {
        /* Parse string to block tree. */
        var script = Engine.Parser.Parse(@"
            <xml>
                <block type=""math_single"">
                    <field name=""OP"">ROOT</field>
                    <value name=""NUM"">
                    <shadow type=""math_number"">
                        <field name=""NUM"">9</field>
                    </shadow>
                    </value>
                </block>        
            </xml>
        ");

        /* Execute the block tree. */
        Assert.That(await script.Run(Site.Object), Is.EqualTo(3));
    }

    /// <summary>
    /// See if we can execute a manually created block tree for a square root.
    /// </summary>
    [Test]
    public async Task Can_Get_Square_Root_As_Block()
    {
        /* Manually create the block tree. */
        var block = new MathSingle
        {
            Fields = { new() { Name = "OP", Value = "ROOT" } },
            Values = { new() { Name = "NUM", Block = CreateNumberBlock("9") } }
        };

        /* Execute the block tree. */
        Assert.That(await block.Evaluate(new Context(Site.Object)), Is.EqualTo(3));
    }
}