using NUnit.Framework;

namespace BlocklyNetTests.Engine;

[TestFixture]
public class GroupAnalyserTests : TestEnvironment
{
    private const string Script1 = @"
        <xml xmlns=""https://developers.google.com/blockly/xml"">
        <block type=""execute_group"" id=""T%p8jx/o/QA6HegV]o:9"" x=""125"" y=""75"">
            <field name=""NAME"">Start it all</field>
            <field name=""RESULT"">Result</field>
            <value name=""RESULT"">
            <block type=""group_execution_result"" id=""I[%_mkd9HSpe3K9^!gg("">
                <field name=""Type"">Type</field>
                <field name=""Result"">Result</field>
                <value name=""Type"">
                <shadow type=""group_execution_result_type"" id=""|5/]Bs6_2AoOEAkha18K"">
                    <field name=""VALUE"">Succeeded</field>
                </shadow>
                </value>
                <value name=""Result"">
                <block type=""text"" id=""^Q]vG*2T0}N^hL2)|V{H"">
                    <field name=""TEXT"">1</field>
                </block>
                </value>
            </block>
            </value>
            <next>
            <block type=""execute_group"" id=""At1{sV=Jj]8Y[J_$[i,O"">
                <field name=""NAME"">Functions</field>
                <field name=""RESULT"">Result</field>
                <value name=""RESULT"">
                <block type=""procedures_callreturn"" id=""nyM(=w~1AK!y:gPB:W}q"">
                    <mutation name=""func1""></mutation>
                </block>
                </value>
                <next>
                <block type=""execute_group"" id=""vYHa5_xGpuCY%H,,KX)n"">
                    <field name=""NAME"">End it all</field>
                    <field name=""RESULT"">Result</field>
                    <value name=""RESULT"">
                    <block type=""group_execution_result"" id=""?2si)*W?}FYd0Sm!_NS*"">
                        <field name=""Type"">Type</field>
                        <field name=""Result"">Result</field>
                        <value name=""Type"">
                        <shadow type=""group_execution_result_type"" id=""I@Rg`jDlM0xC!SyN8htq"">
                            <field name=""VALUE"">Succeeded</field>
                        </shadow>
                        </value>
                        <value name=""Result"">
                        <block type=""text"" id=""TC-p|6:-96.ycayK+$.u"">
                            <field name=""TEXT"">3</field>
                        </block>
                        </value>
                    </block>
                    </value>
                </block>
                </next>
            </block>
            </next>
        </block>
        <block type=""procedures_defreturn"" id=""@(jK/]}^:m1]yUcVV#`o"" x=""1075"" y=""75"">
            <field name=""NAME"">func2</field>
            <comment pinned=""false"" h=""80"" w=""160"">Describe this function...</comment>
            <statement name=""STACK"">
            <block type=""execute_group"" id=""LQmghgu`16o;h6W:MY__"">
                <field name=""NAME"">Inner Func</field>
                <field name=""RESULT"">Result</field>
                <value name=""RESULT"">
                <block type=""group_execution_result"" id=""r1qz4JFJLGp57^AFmTy/"">
                    <field name=""Type"">Type</field>
                    <field name=""Result"">Result</field>
                    <value name=""Type"">
                    <shadow type=""group_execution_result_type"" id=""%1k-H:^tlc_bvX*0:Mc$"">
                        <field name=""VALUE"">Succeeded</field>
                    </shadow>
                    </value>
                    <value name=""Result"">
                    <block type=""text"" id=""_S2yF^V;.]]5fwFY;e?]"">
                        <field name=""TEXT"">2.2.1</field>
                    </block>
                    </value>
                </block>
                </value>
            </block>
            </statement>
            <value name=""RETURN"">
            <block type=""group_execution_result"" id="")Hd5MCI,m}70leZY~%4v"">
                <field name=""Type"">Type</field>
                <field name=""Result"">Result</field>
                <value name=""Type"">
                <shadow type=""group_execution_result_type"" id=""(:CI_,sf[9*WqRuNvm*f"">
                    <field name=""VALUE"">Succeeded</field>
                </shadow>
                </value>
                <value name=""Result"">
                <block type=""text"" id=""9f~gS0mlqL.`9A!;n?1u"">
                    <field name=""TEXT"">2.2</field>
                </block>
                </value>
            </block>
            </value>
        </block>
        <block type=""procedures_defreturn"" id=""3l+D=-j5.aUBgEW8mfz7"" x=""825"" y=""325"">
            <field name=""NAME"">func1</field>
            <comment pinned=""false"" h=""80"" w=""160"">Describe this function...</comment>
            <statement name=""STACK"">
            <block type=""execute_group"" id=""U,FO=1c.LrZ(F-[qo~[6"">
                <field name=""NAME"">Outer Func Start</field>
                <field name=""RESULT"">Result</field>
                <value name=""RESULT"">
                <block type=""group_execution_result"" id=""`fr0|s+3xrm@CN*)OHy{"">
                    <field name=""Type"">Type</field>
                    <field name=""Result"">Result</field>
                    <value name=""Type"">
                    <shadow type=""group_execution_result_type"" id=""5)-LFdn:4W}QsRZ7*q:d"">
                        <field name=""VALUE"">Succeeded</field>
                    </shadow>
                    </value>
                    <value name=""Result"">
                    <block type=""text"" id=""HbwvV;~rWs|H1Dg/rnUb"">
                        <field name=""TEXT"">2.1</field>
                    </block>
                    </value>
                </block>
                </value>
                <next>
                <block type=""execute_group"" id=""/;||8fSyn^ZAEqSsJ@$F"">
                    <field name=""NAME"">Call Inner</field>
                    <field name=""RESULT"">Result</field>
                    <value name=""RESULT"">
                    <block type=""procedures_callreturn"" id=""Isro:p?Y@}U,%L2s(%*-"">
                        <mutation name=""func2""></mutation>
                    </block>
                    </value>
                    <next>
                    <block type=""execute_group"" id=""v.+aqR]pxyFqch3fONn^"">
                        <field name=""NAME"">Outer Func End</field>
                        <field name=""RESULT"">Result</field>
                        <value name=""RESULT"">
                        <block type=""group_execution_result"" id=""MiFbibnRW|lk(zly}4mL"">
                            <field name=""Type"">Type</field>
                            <field name=""Result"">Result</field>
                            <value name=""Type"">
                            <shadow type=""group_execution_result_type"" id="",j@*_:Em;W5v;ehy;ae8"">
                                <field name=""VALUE"">Succeeded</field>
                            </shadow>
                            </value>
                            <value name=""Result"">
                            <block type=""text"" id=""Y;@_7o*xi3$5-52eVF#["">
                                <field name=""TEXT"">2.3</field>
                            </block>
                            </value>
                        </block>
                        </value>
                    </block>
                    </next>
                </block>
                </next>
            </block>
            </statement>
            <value name=""RETURN"">
            <block type=""group_execution_result"" id=""~p_Yg)Z.nB}2`_Qn6=K4"">
                <field name=""Type"">Type</field>
                <field name=""Result"">Result</field>
                <value name=""Type"">
                <shadow type=""group_execution_result_type"" id=""[3^=@4-ZkjY1EdSC39SM"">
                    <field name=""VALUE"">Succeeded</field>
                </shadow>
                </value>
                <value name=""Result"">
                <block type=""text"" id="":iT0(0d!(,~d/$0ox?DO"">
                    <field name=""TEXT"">2</field>
                </block>
                </value>
            </block>
            </value>
        </block>
        </xml>
    ";

    [Test]
    public async Task Can_Retrieve_Group_Structure_With_Functions_Async()
    {
        var script = Engine.Parser.Parse(Script1);

        var tree = await script.GetGroupTreeAsync();

        Assert.That(tree, Has.Count.EqualTo(3));

        Assert.Multiple(() =>
        {
            Assert.That(tree[0].Id, Is.EqualTo("T%p8jx/o/QA6HegV]o:9"));
            Assert.That(tree[0].Name, Is.EqualTo("Start it all"));
            Assert.That(tree[0].Children, Has.Count.EqualTo(0));

            Assert.That(tree[1].Id, Is.EqualTo("At1{sV=Jj]8Y[J_$[i,O"));
            Assert.That(tree[1].Name, Is.EqualTo("Functions"));
            Assert.That(tree[1].Children, Has.Count.EqualTo(3));

            Assert.That(tree[2].Id, Is.EqualTo("vYHa5_xGpuCY%H,,KX)n"));
            Assert.That(tree[2].Name, Is.EqualTo("End it all"));
            Assert.That(tree[2].Children, Has.Count.EqualTo(0));
        });

        Assert.Multiple(() =>
        {
            Assert.That(tree[1].Children[0].Id, Is.EqualTo("U,FO=1c.LrZ(F-[qo~[6"));
            Assert.That(tree[1].Children[0].Name, Is.EqualTo("Outer Func Start"));
            Assert.That(tree[1].Children[0].Children, Has.Count.EqualTo(0));

            Assert.That(tree[1].Children[1].Id, Is.EqualTo("/;||8fSyn^ZAEqSsJ@$F"));
            Assert.That(tree[1].Children[1].Name, Is.EqualTo("Call Inner"));
            Assert.That(tree[1].Children[1].Children, Has.Count.EqualTo(1));

            Assert.That(tree[1].Children[2].Id, Is.EqualTo("v.+aqR]pxyFqch3fONn^"));
            Assert.That(tree[1].Children[2].Name, Is.EqualTo("Outer Func End"));
            Assert.That(tree[1].Children[2].Children, Has.Count.EqualTo(0));
        });

        Assert.Multiple(() =>
        {
            Assert.That(tree[1].Children[1].Children[0].Id, Is.EqualTo("LQmghgu`16o;h6W:MY__"));
            Assert.That(tree[1].Children[1].Children[0].Name, Is.EqualTo("Inner Func"));
            Assert.That(tree[1].Children[1].Children[0].Children, Has.Count.EqualTo(0));
        });
    }
}