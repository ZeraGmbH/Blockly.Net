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

    [Test]
    public async Task Can_Calculate_Sin()
    {
        var script = Engine.Parser.Parse(@"
            <xml>
            <block type=""math_trig"">
                <field name=""OP"">SIN</field>
                <value name=""NUM"">
                <shadow type=""math_number"">
                    <field name=""NUM"">45</field>
                </shadow>
                </value>
            </block>        
            </xml>
        ");

        Assert.That(await script.Run(Site.Object), Is.EqualTo(Math.Sin(Math.PI / 4)));
    }

    [TestCase("E", Math.E)]
    [TestCase("INFINITY", double.PositiveInfinity)]
    [TestCase("PI", Math.PI)]
    public async Task Can_Supply_Constant(string name, double expected)
    {
        var script = Engine.Parser.Parse(@"
            <xml>
            <block type=""math_constant"">
                <field name=""CONSTANT"">$$CONSTANT$$</field>
            </block>        
            </xml>
        ".Replace("$$CONSTANT$$", name));

        Assert.That(await script.Run(Site.Object), Is.EqualTo(expected));
    }

    [Test]
    public async Task Can_Detect_Even_Number()
    {
        var script = Engine.Parser.Parse(@"
            <xml>
            <block type=""math_number_property"">
                <mutation divisor_input=""false""></mutation>
                <field name=""PROPERTY"">EVEN</field>
                <value name=""NUMBER_TO_CHECK"">
                <shadow type=""math_number"">
                    <field name=""NUM"">4</field>
                </shadow>
                </value>
            </block>
            </xml>
       ");

        Assert.That(await script.Run(Site.Object), Is.True);
    }

    [Test]
    public async Task Can_Detect_Odd_Number()
    {
        var script = Engine.Parser.Parse(@"
            <xml>
            <block type=""math_number_property"">
                <mutation divisor_input=""false""></mutation>
                <field name=""PROPERTY"">ODD</field>
                <value name=""NUMBER_TO_CHECK"">
                <shadow type=""math_number"">
                    <field name=""NUM"">3</field>
                </shadow>
                </value>
            </block>
            </xml>
        ");

        Assert.That(await script.Run(Site.Object), Is.True);
    }

    [Test]
    public async Task Can_Detect_Prime()
    {
        var script = Engine.Parser.Parse(@"
            <xml>
            <block type=""math_number_property"">
                <mutation divisor_input=""false""></mutation>
                <field name=""PROPERTY"">PRIME</field>
                <value name=""NUMBER_TO_CHECK"">
                <shadow type=""math_number"">
                    <field name=""NUM"">29</field>
                </shadow>
                </value>
            </block>
            </xml>
        ");

        Assert.That(await script.Run(Site.Object), Is.True);
    }

    [TestCase("7", true)]
    [TestCase("7.1", false)]
    public async Task Can_Detect_Whole_Number(string number, bool expected)
    {
        var script = Engine.Parser.Parse(@"
            <xml>
            <block type=""math_number_property"">
                <mutation divisor_input=""false""></mutation>
                <field name=""PROPERTY"">WHOLE</field>
                <value name=""NUMBER_TO_CHECK"">
                <shadow type=""math_number"">
                    <field name=""NUM"">$$NUMBER$$</field>
                </shadow>
                </value>
            </block>
            </xml>
        ".Replace("$$NUMBER$$", number));

        Assert.That(await script.Run(Site.Object), Is.EqualTo(expected));
    }

    [Test]
    public async Task Can_Detect_Positive_Number()
    {
        var script = Engine.Parser.Parse(@"
            <xml>
            <block type=""math_number_property"">
                <mutation divisor_input=""false""></mutation>
                <field name=""PROPERTY"">POSITIVE</field>
                <value name=""NUMBER_TO_CHECK"">
                <shadow type=""math_number"">
                    <field name=""NUM"">7.1</field>
                </shadow>
                </value>
            </block>
            </xml>
        ");

        Assert.That(await script.Run(Site.Object), Is.True);
    }

    [Test]
    public async Task Can_Detect_Negative_Number()
    {
        var script = Engine.Parser.Parse(@"
            <xml>
            <block type=""math_number_property"">
                <mutation divisor_input=""false""></mutation>
                <field name=""PROPERTY"">NEGATIVE</field>
                <value name=""NUMBER_TO_CHECK"">
                <shadow type=""math_number"">
                    <field name=""NUM"">7.1</field>
                </shadow>
                </value>
            </block>
            </xml>
        ");

        Assert.That(await script.Run(Site.Object), Is.False);
    }

    [Test]
    public async Task Can_Test_For_Divisible_By()
    {
        var script = Engine.Parser.Parse(@"
            <xml>
            <block type=""math_number_property"">
                <mutation divisor_input=""true""></mutation>
                <field name=""PROPERTY"">DIVISIBLE_BY</field>
                <value name=""NUMBER_TO_CHECK"">
                <shadow type=""math_number"">
                    <field name=""NUM"">9</field>
                </shadow>
                </value>
                <value name=""DIVISOR"">
                <block type=""math_number"">
                    <field name=""NUM"">3</field>
                </block>
                </value>
            </block>
            </xml>
        ");

        Assert.That(await script.Run(Site.Object), Is.True);
    }

    [Test]
    public async Task Can_Round()
    {
        var script = Engine.Parser.Parse(@"
            <xml>
            <block type=""math_round"">
                <field name=""OP"">ROUND</field>
                <value name=""NUM"">
                <shadow type=""math_number"">
                    <field name=""NUM"">3.1</field>
                </shadow>
                </value>
            </block>
            </xml>
        ");

        Assert.That(await script.Run(Site.Object), Is.EqualTo(3.0));
    }

    [Test]
    public async Task Can_Round_Up()
    {
        var script = Engine.Parser.Parse(@"
            <xml>
            <block type=""math_round"">
                <field name=""OP"">ROUNDUP</field>
                <value name=""NUM"">
                <shadow type=""math_number"">
                    <field name=""NUM"">3.1</field>
                </shadow>
                </value>
            </block>
            </xml>
        ");

        Assert.That(await script.Run(Site.Object), Is.EqualTo(4.0));
    }

    [Test]
    public async Task Can_Round_Down()
    {
        var script = Engine.Parser.Parse(@"
            <xml>
            <block type=""math_round"">
                <field name=""OP"">ROUNDDOWN</field>
                <value name=""NUM"">
                <shadow type=""math_number"">
                    <field name=""NUM"">3.1</field>
                </shadow>
                </value>
            </block>
            </xml>
        ");

        Assert.That(await script.Run(Site.Object), Is.EqualTo(3.0));
    }

    [Test]
    public async Task Can_Sum_List()
    {
        var script = Engine.Parser.Parse(@"
            <xml xmlns=""http://www.w3.org/1999/xhtml"">
            <block type=""math_on_list"">
                <mutation op=""SUM""></mutation>
                <field name=""OP"">SUM</field>
                <value name=""LIST"">
                <block type=""lists_repeat"">
                    <value name=""ITEM"">
                    <block type=""math_number"">
                        <field name=""NUM"">3</field>
                    </block>
                    </value>
                    <value name=""NUM"">
                    <shadow type=""math_number"">
                        <field name=""NUM"">5</field>
                    </shadow>
                    </value>
                </block>
                </value>
            </block>
            </xml>
        ");

        Assert.That(await script.Run(Site.Object), Is.EqualTo(15));
    }

    [Test]
    public async Task Can_Choose_Random_List_Element()
    {
        var script = Engine.Parser.Parse(@"
            <xml xmlns=""http://www.w3.org/1999/xhtml"">
            <block type=""math_on_list"">
                <mutation op=""RANDOM""></mutation>
                <field name=""OP"">RANDOM</field>
                <value name=""LIST"">
                <block type=""lists_repeat"">
                    <value name=""ITEM"">
                    <block type=""math_number"">
                        <field name=""NUM"">3</field>
                    </block>
                    </value>
                    <value name=""NUM"">
                    <shadow type=""math_number"">
                        <field name=""NUM"">5</field>
                    </shadow>
                    </value>
                </block>
                </value>
            </block>
            </xml>
        ");

        Assert.That(await script.Run(Site.Object), Is.EqualTo(3));
    }

    [Test]
    public async Task Can_Group_On_List()
    {
        var script = Engine.Parser.Parse(@"
            <xml xmlns=""http://www.w3.org/1999/xhtml"">
            <block type=""math_on_list"">
                <mutation op=""MODE""></mutation>
                <field name=""OP"">MODE</field>
                <value name=""LIST"">
                <block type=""lists_repeat"">
                    <value name=""ITEM"">
                    <block type=""math_number"">
                        <field name=""NUM"">3</field>
                    </block>
                    </value>
                    <value name=""NUM"">
                    <shadow type=""math_number"">
                        <field name=""NUM"">5</field>
                    </shadow>
                    </value>
                </block>
                </value>
            </block>
            </xml>
        ");

        Assert.That(await script.Run(Site.Object), Is.EqualTo(3));
    }

    [Test]
    public async Task Can_Contrain()
    {
        var script = Engine.Parser.Parse(@"
            <xml xmlns=""http://www.w3.org/1999/xhtml"">
            <block type=""math_constrain"">
                <value name=""VALUE"">
                <shadow type=""math_number"">
                    <field name=""NUM"">110</field>
                </shadow>
                </value>
                <value name=""LOW"">
                <shadow type=""math_number"">
                    <field name=""NUM"">1</field>
                </shadow>
                </value>
                <value name=""HIGH"">
                <shadow type=""math_number"">
                    <field name=""NUM"">100</field>
                </shadow>
                </value>
            </block>
            </xml>
        ");

        Assert.That(await script.Run(Site.Object), Is.EqualTo(100));
    }

    [Test]
    public async Task Can_Get_Modulo()
    {
        var script = Engine.Parser.Parse(@"
            <xml xmlns=""http://www.w3.org/1999/xhtml"">
            <variables></variables>
            <block type=""math_modulo"">
                <value name=""DIVIDEND"">
                <shadow type=""math_number"">
                    <field name=""NUM"">64</field>
                </shadow>
                </value>
                <value name=""DIVISOR"">
                <shadow type=""math_number"">
                    <field name=""NUM"">10</field>
                </shadow>
                </value>
            </block>
            </xml>
        ");

        Assert.That(await script.Run(Site.Object), Is.EqualTo(4));
    }

    [Test]
    public async Task Can_Generate_Random_Number()
    {
        var script = Engine.Parser.Parse(@"
            <xml xmlns=""http://www.w3.org/1999/xhtml"">
            <variables></variables>
            <block type=""math_random_float""></block>
            </xml>
        ");

        Assert.That(await script.Run(Site.Object), Is.GreaterThanOrEqualTo(0d).And.LessThanOrEqualTo(1d));
    }

    [Test]
    public async Task Can_Generate_Integral_Random_Number()
    {
        var script = Engine.Parser.Parse(@"
            <xml xmlns=""http://www.w3.org/1999/xhtml"">
            <block type=""math_random_int"">
                <value name=""FROM"">
                <shadow type=""math_number"">
                    <field name=""NUM"">1</field>
                </shadow>
                </value>
                <value name=""TO"">
                <shadow type=""math_number"">
                    <field name=""NUM"">100</field>
                </shadow>
                </value>
            </block>
            </xml>
        ");

        Assert.That(await script.Run(Site.Object), Is.GreaterThanOrEqualTo(1).And.LessThanOrEqualTo(100));
    }

    [Test]
    public async Task Can_Change_Number_Variable()
    {
        var script = Engine.Parser.Parse(@"
            <xml xmlns=""https://developers.google.com/blockly/xml"">
            <variables>
                <variable id=""ff`YJBi(D@smL[)Q:H}}"">foo</variable>
            </variables>
            <block type=""variables_set"" id=""a2aSu{9x7/D~9z3WGU(C"" x=""562"" y=""112"">
                <field name=""VAR"" id=""ff`YJBi(D@smL[)Q:H}}"">foo</field>
                <value name=""VALUE"">
                <block type=""math_number"" id=""(=?Qs,U~+c+hewlZejLb"">
                    <field name=""NUM"">1</field>
                </block>
                </value>
                <next>
                    <block type=""math_change"" id=""uO~$6GN{K~{gOBd!r%vp"">
                        <field name=""VAR"" id=""ff`YJBi(D@smL[)Q:H}}"">foo</field>
                        <value name=""DELTA"">
                        <shadow type=""math_number"" id=""S3n?jRy1.r1?+xGsN[ba"">
                            <field name=""NUM"">1</field>
                        </shadow>
                        </value>
                        <next>                   
                            <block type=""variables_get"" id="")[}s1A^ZtI^hEMi5qjuw"">
                                <field name=""VAR"" id=""ff`YJBi(D@smL[)Q:H}}"">foo</field>
                            </block>
                        </next>
                    </block>
                </next>
            </block>
            </xml>");

        Assert.That(await script.Run(Site.Object), Is.EqualTo(2d));
    }
}