using NUnit.Framework;

namespace BlocklyNetTests.Core;

[TestFixture]
public class ControlTests : TestEnvironment
{
    [Test]
    public async Task Can_Execute_Simple_If()
    {
        var script = Engine.Parser.Parse(@"
            <xml>
            <variables>
                <variable type="""">result</variable>
            </variables>            
            <block type=""controls_if"" >
                <value name=""IF0"">
                <block type=""logic_boolean"">
                    <field name=""BOOL"">TRUE</field>
                </block>
                </value>
                <statement name=""DO0"">
                    <block type=""variables_set"">
                        <field name=""VAR"" variabletype="""">result</field>
                        <value name=""VALUE"">
                            <block type=""math_number"">
                                <field name=""NUM"">1</field>
                            </block>
                        </value>                
                    </block>
                </statement>
                <next>
                    <block type=""variables_get"">
                        <field name=""VAR"" variabletype="""">result</field>
                    </block>            
                </next>
            </block>       
            </xml>");

        Assert.That(await script.Run(Site.Object), Is.EqualTo(1));
    }


    [Test]
    public async Task Can_Execute_WhileUntil()
    {
        var script = Engine.Parser.Parse(@"
            <xml xmlns=""http://www.w3.org/1999/xhtml"">
            <variables>
                <variable type="""">x</variable>
            </variables>
            <block type=""variables_set"">
                <field name=""VAR"" variabletype="""">x</field>
                <value name=""VALUE"">
                <block type=""math_number"">
                    <field name=""NUM"">0</field>
                </block>
                </value>
                <next>
                <block type=""controls_whileUntil"">
                    <field name=""MODE"">WHILE</field>
                    <value name=""BOOL"">
                    <block type=""logic_compare"">
                        <field name=""OP"">EQ</field>
                        <value name=""A"">
                        <block type=""variables_get"">
                            <field name=""VAR"" variabletype="""">x</field>
                        </block>
                        </value>
                        <value name=""B"">
                        <block type=""math_number"">
                            <field name=""NUM"">0</field>
                        </block>
                        </value>
                    </block>
                    </value>
                    <statement name=""DO"">
                    <block type=""variables_set"">
                        <field name=""VAR"" variabletype="""">x</field>
                        <value name=""VALUE"">
                        <block type=""math_number"">
                            <field name=""NUM"">1</field>
                        </block>
                        </value>
                    </block>
                    </statement>
                    <next>
                        <block type=""variables_get"">
                            <field name=""VAR"" variabletype="""">x</field>
                        </block>            
                    </next>
                </block>
                </next>
            </block>
            </xml>");

        Assert.That(await script.Run(Site.Object), Is.EqualTo(1d));
    }

    [Test]
    public async Task Can_Continue_In_Lopp()
    {
        var script = Engine.Parser.Parse(@"
            <xml>
            <variables>
                <variable type="""">result</variable>
            </variables>
            <block type=""variables_set"">
                <field name=""VAR"">result</field>
                <value name=""VALUE"">
                    <block type=""math_number"">
                        <field name=""NUM"">0</field>
                    </block>
                </value>
                <next>
                    <block type=""controls_repeat_ext"">
                        <value name=""TIMES"">
                            <shadow type=""math_number"">
                                <field name=""NUM"">3</field>
                            </shadow>
                        </value>
                        <statement name=""DO"">
                        <block type=""controls_if"">
                            <value name=""IF0"">
                            <block type=""logic_boolean"">
                                <field name=""BOOL"">TRUE</field>
                            </block>
                            </value>
                            <statement name=""DO0"">
                            <block type=""controls_flow_statements"">
                                <field name=""FLOW"">CONTINUE</field>
                            </block>
                            </statement>
                            <next>
                                <block type=""math_change"">
                                    <field name=""VAR"">result</field>
                                    <value name=""DELTA"">
                                        <shadow type=""math_number"">
                                            <field name=""NUM"">1</field>
                                        </shadow>
                                    </value>
                                </block>
                            </next>
                        </block>
                        </statement>
                        <next>
                            <block type=""variables_get"">
                                <field name=""VAR"" variabletype="""">result</field>
                            </block>            
                        </next>
                    </block>
                </next>
            </block>
            </xml>");

        Assert.That(await script.Run(Site.Object), Is.EqualTo(0d));
    }

    [Test]
    public async Task Can_Break_Loop()
    {
        var script = Engine.Parser.Parse(@"
            <xml>
            <variables>
                <variable type="""">result</variable>
            </variables>
            <block type=""variables_set"">
                <field name=""VAR"">result</field>
                <value name=""VALUE"">
                    <block type=""math_number"">
                        <field name=""NUM"">0</field>
                    </block>
                </value>
                <next>
                    <block type=""controls_repeat_ext"">
                        <value name=""TIMES"">
                        <shadow type=""math_number"">
                            <field name=""NUM"">3</field>
                        </shadow>
                        </value>
                        <statement name=""DO"">
                        <block type=""controls_if"">
                            <value name=""IF0"">
                            <block type=""logic_boolean"">
                                <field name=""BOOL"">TRUE</field>
                            </block>
                            </value>
                            <statement name=""DO0"">
                            <block type=""controls_flow_statements"">
                                <field name=""FLOW"">BREAK</field>
                            </block>
                            </statement>
                            <next>
                                <block type=""math_change"">
                                    <field name=""VAR"">result</field>
                                    <value name=""DELTA"">
                                        <shadow type=""math_number"">
                                            <field name=""NUM"">1</field>
                                        </shadow>
                                    </value>
                                </block>
                            </next>
                        </block>
                        </statement>
                        <next>
                            <block type=""variables_get"">
                                <field name=""VAR"" variabletype="""">result</field>
                            </block>            
                        </next>
                    </block>
                </next>
            </block>
            </xml>");

        Assert.That(await script.Run(Site.Object), Is.EqualTo(0d));

    }

    [Test]
    public async Task Can_Execute_For_Each()
    {
        var script = Engine.Parser.Parse(@"
            <xml xmlns=""http://www.w3.org/1999/xhtml"">
            <variables>
                <variable type="""">i</variable>
                <variable type="""">result</variable>
            </variables>
            <block type=""variables_set"">
                <field name=""VAR"">result</field>
                <value name=""VALUE"">
                    <block type=""text"">
                        <field name=""TEXT""></field>
                    </block>
                </value>
                <next>
                    <block type=""controls_forEach"">
                        <field name=""VAR"" variabletype="""">i</field>
                        <value name=""LIST"">
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
                        <statement name=""DO"">
                            <block type=""text_append"">
                                <field name=""VAR"">result</field>
                                <value name=""TEXT"">
                                    <block type=""variables_get"">
                                        <field name=""VAR"" variabletype="""">i</field>
                                    </block>            
                                </value>
                            </block>
                        </statement>
                        <next>
                            <block type=""variables_get"">
                                <field name=""VAR"" variabletype="""">result</field>
                            </block>            
                        </next>
                    </block>
               </next>
            </block>
            </xml>");

        Assert.That(await script.Run(Site.Object), Is.EqualTo("abc"));
    }

    [Test]
    public async Task Can_Loop()
    {
        var script = Engine.Parser.Parse(@"
            <xml xmlns=""http://www.w3.org/1999/xhtml"">
            <variables>
                <variable type="""">i</variable>
            </variables>
            <block type=""controls_for"">
                <field name=""VAR"" variabletype="""">i</field>
                <value name=""FROM"">
                <shadow type=""math_number"">
                    <field name=""NUM"">1</field>
                </shadow>
                </value>
                <value name=""TO"">
                <shadow type=""math_number"">
                    <field name=""NUM"">3</field>
                </shadow>
                </value>
                <value name=""BY"">
                <shadow type=""math_number"">
                    <field name=""NUM"">1</field>
                </shadow>
                </value>
                <statement name=""DO"">
                    <block type=""variables_get"">
                        <field name=""VAR"" variabletype="""">i</field>
                    </block>
                </statement>
                <next>
                    <block type=""variables_get"">
                        <field name=""VAR"" variabletype="""">i</field>
                    </block>
                </next>
            </block>
            </xml>");

        Assert.That(await script.Run(Site.Object), Is.EqualTo(4));
    }
}