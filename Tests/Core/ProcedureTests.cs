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

    [Test]
    public async Task Can_If_Return_Even_If_Nested_Async()
    {
        var script = Engine.Parser.Parse(@"                    
            <xml xmlns=""https://developers.google.com/blockly/xml"">
            <variables>
                <variable id=""wCdL(6[3c{AdDw)V2tql"">mode</variable>
                <variable id=""ax(L4U[~k^K;#9_qv.w="">i</variable>
            </variables>
            <block type=""lists_create_with"" id=""umlji;F=;fwjkCQ~sf^4"" x=""125"" y=""75"">
                <mutation items=""4""></mutation>
                <value name=""ADD0"">
                <block type=""procedures_callreturn"" id=""}(7WgmL+xpU[F}`kCw*+"">
                    <mutation name=""theProcedureWithReturn"">
                    <arg name=""mode""></arg>
                    </mutation>
                    <value name=""ARG0"">
                    <block type=""math_number"" id=""H[vmrcbXbr!oA,`N{-/~"">
                        <field name=""NUM"">1</field>
                    </block>
                    </value>
                </block>
                </value>
                <value name=""ADD1"">
                <block type=""procedures_callreturn"" id=""y3ljjM]Ty*2eZa%$x8Eg"">
                    <mutation name=""theProcedureWithReturn"">
                    <arg name=""mode""></arg>
                    </mutation>
                    <value name=""ARG0"">
                    <block type=""math_number"" id=""l/S@(^d]w}x0c=rq6P;@"">
                        <field name=""NUM"">2</field>
                    </block>
                    </value>
                </block>
                </value>
                <value name=""ADD2"">
                <block type=""procedures_callreturn"" id=""K}KogpC:r{5#dTp8FGnv"">
                    <mutation name=""theProcedureWithReturn"">
                    <arg name=""mode""></arg>
                    </mutation>
                    <value name=""ARG0"">
                    <block type=""math_number"" id=""$r?_Zogkw1|LuVIJxVdP"">
                        <field name=""NUM"">3</field>
                    </block>
                    </value>
                </block>
                </value>
                <value name=""ADD3"">
                <block type=""procedures_callreturn"" id=""ZgZWmHahckgG].p9J+z$"">
                    <mutation name=""theProcedureWithReturn"">
                    <arg name=""mode""></arg>
                    </mutation>
                    <value name=""ARG0"">
                    <block type=""math_number"" id=""`mnOP8%::uy^~JoIZ}d`"">
                        <field name=""NUM"">4</field>
                    </block>
                    </value>
                </block>
                </value>
            </block>
            <block type=""procedures_defreturn"" id=""D.TnGA?$_.W7b-0]rZU7"" x=""825"" y=""75"">
                <mutation>
                <arg name=""mode"" varid=""wCdL(6[3c{AdDw)V2tql""></arg>
                </mutation>
                <field name=""NAME"">theProcedureWithReturn</field>
                <comment pinned=""false"" h=""80"" w=""160"">Beschreibe diese Funktion …</comment>
                <statement name=""STACK"">
                <block type=""procedures_ifreturn"" id=""TkV9G(#aET!?(kGJ*?Qo"">
                    <mutation value=""1""></mutation>
                    <value name=""CONDITION"">
                    <block type=""logic_compare"" id=""Q|%Gz-[h(@I|plwf0)NO"">
                        <field name=""OP"">EQ</field>
                        <value name=""A"">
                        <block type=""variables_get"" id=""D0f-3VMI*b`XUsAr;Z9Z"">
                            <field name=""VAR"" id=""wCdL(6[3c{AdDw)V2tql"">mode</field>
                        </block>
                        </value>
                        <value name=""B"">
                        <block type=""math_number"" id=""iV[jvGh?wwk|LsvKFPeG"">
                            <field name=""NUM"">1</field>
                        </block>
                        </value>
                    </block>
                    </value>
                    <value name=""VALUE"">
                    <block type=""math_number"" id=""x^frQL/pRWTl%t`0a}_r"">
                        <field name=""NUM"">1</field>
                    </block>
                    </value>
                    <next>
                    <block type=""controls_if"" id=""6~Mh?:6Kp#Nmzw2,7r]d"">
                        <value name=""IF0"">
                        <block type=""logic_compare"" id=""AH@xm{H+.QZ2s*K=$e8_"">
                            <field name=""OP"">NEQ</field>
                            <value name=""A"">
                            <block type=""variables_get"" id=""hZ,++8e)HU3]2XyYI(@3"">
                                <field name=""VAR"" id=""wCdL(6[3c{AdDw)V2tql"">mode</field>
                            </block>
                            </value>
                            <value name=""B"">
                            <block type=""math_number"" id=""3JfDG/u*Ga1ER7){r/q+"">
                                <field name=""NUM"">1</field>
                            </block>
                            </value>
                        </block>
                        </value>
                        <statement name=""DO0"">
                        <block type=""controls_for"" id=""z(C+uHUnY7Q)#XY9|hUF"">
                            <field name=""VAR"" id=""ax(L4U[~k^K;#9_qv.w="">i</field>
                            <value name=""FROM"">
                            <shadow type=""math_number"" id=""[sATlyi[0H.*.;dH4s`}"">
                                <field name=""NUM"">1</field>
                            </shadow>
                            </value>
                            <value name=""TO"">
                            <shadow type=""math_number"" id=""nnyKp#4y[ug}M$ADSOXd"">
                                <field name=""NUM"">10</field>
                            </shadow>
                            </value>
                            <value name=""BY"">
                            <shadow type=""math_number"" id=""S[=Sx~=oHnP5Y,Y7riPz"">
                                <field name=""NUM"">1</field>
                            </shadow>
                            </value>
                            <statement name=""DO"">
                            <block type=""procedures_ifreturn"" id=""$:Rds*B5NCJMTCo1T4un"">
                                <mutation value=""1""></mutation>
                                <value name=""CONDITION"">
                                <block type=""logic_compare"" id=""urhEu1MXS;iX0opNqqM!"">
                                    <field name=""OP"">EQ</field>
                                    <value name=""A"">
                                    <block type=""variables_get"" id=""syrf_Fbt$fkl:lwVOnBJ"">
                                        <field name=""VAR"" id=""wCdL(6[3c{AdDw)V2tql"">mode</field>
                                    </block>
                                    </value>
                                    <value name=""B"">
                                    <block type=""math_number"" id=""iW5oBY[@y:{*AF|s4Er2"">
                                        <field name=""NUM"">2</field>
                                    </block>
                                    </value>
                                </block>
                                </value>
                                <value name=""VALUE"">
                                <block type=""math_number"" id=""}3(K~h8q^96@^,r;X~!."">
                                    <field name=""NUM"">2</field>
                                </block>
                                </value>
                                <next>
                                <block type=""controls_if"" id=""-8{$!Ld+Bh5m-Wb!E+~f"">
                                    <value name=""IF0"">
                                    <block type=""logic_compare"" id=""e(Ca4K7n}l4N;N*.%|/a"">
                                        <field name=""OP"">NEQ</field>
                                        <value name=""A"">
                                        <block type=""variables_get"" id=""H-Qwh3[=qpKil=dW5QPc"">
                                            <field name=""VAR"" id=""wCdL(6[3c{AdDw)V2tql"">mode</field>
                                        </block>
                                        </value>
                                        <value name=""B"">
                                        <block type=""math_number"" id=""QdZGc+Ox=K,I`_y*!vmh"">
                                            <field name=""NUM"">2</field>
                                        </block>
                                        </value>
                                    </block>
                                    </value>
                                    <statement name=""DO0"">
                                    <block type=""try_catch_finally"" id=""G-[$;GvOg{G?gvzc#X;5"">
                                        <statement name=""TRY"">
                                        <block type=""procedures_ifreturn"" id=""7-x-~*#PB~lCB_qK2~S4"">
                                            <mutation value=""1""></mutation>
                                            <value name=""CONDITION"">
                                            <block type=""logic_compare"" id=""uA5?_wO{nRuB|pOoE(]k"">
                                                <field name=""OP"">EQ</field>
                                                <value name=""A"">
                                                <block type=""variables_get"" id=""r9szc|$s_QwxCgWIm+8y"">
                                                    <field name=""VAR"" id=""wCdL(6[3c{AdDw)V2tql"">mode</field>
                                                </block>
                                                </value>
                                                <value name=""B"">
                                                <block type=""math_number"" id=""Zu^]JKU+`tymD)f8S(Q_"">
                                                    <field name=""NUM"">3</field>
                                                </block>
                                                </value>
                                            </block>
                                            </value>
                                            <value name=""VALUE"">
                                            <block type=""math_number"" id=""jM6q0X#kdezp|,:hnK#d"">
                                                <field name=""NUM"">3</field>
                                            </block>
                                            </value>
                                        </block>
                                        </statement>
                                    </block>
                                    </statement>
                                </block>
                                </next>
                            </block>
                            </statement>
                        </block>
                        </statement>
                    </block>
                    </next>
                </block>
                </statement>
                <value name=""RETURN"">
                <block type=""text"" id=""=1@f,^`ZGP%:7XXqrB(-"">
                    <field name=""TEXT"">Default</field>
                </block>
                </value>
            </block>
            </xml>");

        var results = (IList<object?>)(await script.RunAsync(Site.Object))!;

        Assert.That(results, Has.Count.EqualTo(4));

        Assert.Multiple(() =>
        {
            Assert.That(results[0], Is.EqualTo(1));
            Assert.That(results[1], Is.EqualTo(2));
            Assert.That(results[2], Is.EqualTo(3));
            Assert.That(results[3], Is.EqualTo("Default"));
        });
    }
}
