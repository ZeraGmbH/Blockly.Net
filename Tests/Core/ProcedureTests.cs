using NUnit.Framework;

namespace BlocklyNetTests.Core;

[TestFixture]
public class ProcedureTests : TestEnvironment
{
    [Test]
    public async Task Can_Execute_Procedure_Async()
    {
        var script = Engine.Parser.Parse(@"
            <xml xmlns=""http://www.w3.org/1999/xhtml"">
            <variables>
                <variable type="""">x</variable>
                <variable type="""">result</variable>
            </variables>
            <block type=""procedures_defnoreturn"">
                <mutation>
                <arg name=""x""></arg>
                </mutation>
                <field name=""NAME"">do something</field>
                <comment pinned=""false"" h=""80"" w=""160"">Describe this function...</comment>
                <statement name=""STACK"">
                    <block type=""variables_set"">
                        <field name=""VAR"">result</field>
                        <value name=""VALUE"">
                            <block type=""text_join"">
                                <mutation items=""3""></mutation>
                                <value name=""ADD0"">
                                    <block type=""variables_get"" >
                                        <field name=""VAR"" variabletype="""">result</field>
                                    </block>
                                </value>
                                <value name=""ADD1"">
                                    <block type=""text"">
                                        <field name=""TEXT"">,</field>
                                    </block>
                                </value>
                                <value name=""ADD2"">
                                    <block type=""variables_get"" >
                                        <field name=""VAR"" variabletype="""">x</field>
                                    </block>
                                </value>
                            </block>
                        </value>
                    </block>                
                </statement>
            </block>
            <block type=""variables_set"">
                <field name=""VAR"">result</field>
                <value name=""VALUE"">
                    <block type=""text"">
                        <field name=""TEXT""></field>
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
                            <block type=""procedures_callnoreturn"">
                                <mutation name=""do something"">
                                <arg name=""x""></arg>
                                </mutation>
                                <value name=""ARG0"">
                                <block type=""text"">
                                    <field name=""TEXT"">hello world</field>
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

        Assert.That(await script.RunAsync(Site.Object), Is.EqualTo(",hello world,hello world,hello world"));
    }

    [Test]
    public async Task Can_Return_Async()
    {
        var script = Engine.Parser.Parse(@"
            <xml xmlns=""http://www.w3.org/1999/xhtml"">
            <block type=""procedures_defreturn"">
                <field name=""NAME"">do something</field>
                <comment pinned=""false"" h=""80"" w=""160"">Describe this function...</comment>
                <value name=""RETURN"">
                <block type=""text"" id=""4p1uAONhYe8wWJ};60Ff"">
                    <field name=""TEXT"">hello world</field>
                </block>
                </value>
            </block>
            <block type=""procedures_callreturn"" id=""%qnT~o/4TK+nMt-tCrh6"" x=""238"" y=""113"">
                <mutation name=""do something""></mutation>
            </block>
            </xml>");

        Assert.That(await script.RunAsync(Site.Object), Is.EqualTo("hello world"));
    }

    [Test]
    public async Task Can_If_Return_Async()
    {
        var script = Engine.Parser.Parse(@"
            <xml xmlns=""http://www.w3.org/1999/xhtml"">
            <block type=""procedures_defreturn"">
                <field name=""NAME"">do something</field>
                <comment pinned=""false"">Describe this function...</comment>
                <statement name=""STACK"">
                <block type=""procedures_ifreturn"">
                    <mutation value=""1""></mutation>
                    <value name=""CONDITION"">
                    <block type=""logic_boolean"">
                        <field name=""BOOL"">TRUE</field>
                    </block>
                    </value>
                    <value name=""VALUE"">
                    <block type=""text"">
                        <field name=""TEXT"">hello world</field>
                    </block>
                    </value>
                </block>
                </statement>
                <value name=""RETURN"">
                <block type=""text"">
                    <field name=""TEXT"">xxx</field>
                </block>
                </value>
            </block>
            <block type=""procedures_callreturn"">
                <mutation name=""do something""></mutation>
            </block>
            </xml>");

        Assert.That(await script.RunAsync(Site.Object), Is.EqualTo("hello world"));
    }
}
