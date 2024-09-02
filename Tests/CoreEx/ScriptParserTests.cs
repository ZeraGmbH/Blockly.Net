using BlocklyNet.Core;
using BlocklyNet.Extensions.Builder;
using BlocklyNet.Scripting.Engine;
using BlocklyNet.Scripting.Parsing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace BlocklyNetTests.CoreEx;

[TestFixture]
public class ScriptParserTests
{
    private ServiceProvider Services = null!;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();

        services.AddHttpClient();

        var models = new ScriptModels();

        services.AddTransient(ctx => Parser.CreateService(true, models, null));

        Services = services.BuildServiceProvider();
    }

    [TearDown]
    public void Teardown()
    {
        Services?.Dispose();
    }

    [Test]
    public async Task Can_Parse_And_Compile_Xml_Script_Async()
    {
        const string xml = @"
            <xml xmlns=""https://developers.google.com/blockly/xml"">
                <variables>
                    <variable id=""Q}Tj{.,3Bg3;?kgeY~w,"">X12</variable>
                </variables>
                <block type=""variables_set"" id=""n)/JW#/fseB~G%jp;|-Q"" x=""22"" y=""13"">
                    <field name=""VAR"" id=""Q}Tj{.,3Bg3;?kgeY~w,"">X12</field>
                    <value name=""VALUE"">
                    <block type=""math_number"" id=""Hm(*uYv`Cu09[kN$a)e|"">
                        <field name=""NUM"">9999</field>
                    </block>
                    </value>
                    <next>
                    <block type=""controls_if"" id=""0p^ZJ%6AX-GR[O9m6})w"">
                        <value name=""IF0"">
                        <block type=""logic_compare"" id=""%J4o!NMP#ENfPs*I_NmK"">
                            <field name=""OP"">EQ</field>
                            <value name=""A"">
                            <block type=""variables_get"" id=""vs4^Y19yq|$9mzf?T;~v"">
                                <field name=""VAR"" id=""Q}Tj{.,3Bg3;?kgeY~w,"">X12</field>
                            </block>
                            </value>
                            <value name=""B"">
                            <block type=""math_number"" id=""2$n!{w=@w7tm,Z`].W[e"">
                                <field name=""NUM"">33</field>
                            </block>
                            </value>
                        </block>
                        </value>
                        <statement name=""DO0"">
                        <block type=""procedures_callnoreturn"" id=""J3L^`+/A*#I+mi7U?O%^"">
                            <mutation name=""bad guy""></mutation>
                        </block>
                        </statement>
                    </block>
                    </next>
                </block>
                <block type=""procedures_defnoreturn"" id=""-8M^n_T5|r}r+M-q$FC["" x=""23"" y=""151"">
                    <field name=""NAME"">bad guy</field>
                    <comment pinned=""false"" h=""80"" w=""160"">Does aktually nothing</comment>
                    <statement name=""STACK"">
                    <block type=""controls_if"" id=""k4mj-SXz;,I_gKzv;c:Z"">
                        <mutation elseif=""2"" else=""1""></mutation>
                        <value name=""IF0"">
                        <block type=""logic_compare"" id=""a(;PrN_%?}/Rn9-iS!*I"">
                            <field name=""OP"">LT</field>
                            <value name=""A"">
                            <block type=""variables_get"" id=""tjSQ:Xo~h)ozo`!PfeYi"">
                                <field name=""VAR"" id=""Q}Tj{.,3Bg3;?kgeY~w,"">X12</field>
                            </block>
                            </value>
                            <value name=""B"">
                            <block type=""math_number"" id=""F;0X*t40XmM)RrA{W@sP"">
                                <field name=""NUM"">12</field>
                            </block>
                            </value>
                        </block>
                        </value>
                        <statement name=""DO0"">
                        <block type=""procedures_callnoreturn"" id="")9h6ZhjeB3uh$/!./M:!"">
                            <mutation name=""bad guy""></mutation>
                        </block>
                        </statement>
                        <value name=""IF1"">
                        <block type=""logic_compare"" id=""~WnH%V}.k/CGAjF+ZX|y"">
                            <field name=""OP"">LT</field>
                            <value name=""A"">
                            <block type=""variables_get"" id=""9CuO,740:ZAgL~KaBm0}"">
                                <field name=""VAR"" id=""Q}Tj{.,3Bg3;?kgeY~w,"">X12</field>
                            </block>
                            </value>
                            <value name=""B"">
                            <block type=""math_number"" id="":#Sj1WB]I6L/_+{.f)R`"">
                                <field name=""NUM"">13</field>
                            </block>
                            </value>
                        </block>
                        </value>
                        <statement name=""DO1"">
                        <block type=""procedures_callnoreturn"" id=""eXq/I.bedz9uK0p!;[$9"">
                            <mutation name=""bad guy""></mutation>
                        </block>
                        </statement>
                        <value name=""IF2"">
                        <block type=""logic_compare"" id=""DwHk+,PT[MG.gc5!wy_B"">
                            <field name=""OP"">LT</field>
                            <value name=""A"">
                            <block type=""variables_get"" id=""%`#j4rtfha8Jwj8Uptp/"">
                                <field name=""VAR"" id=""Q}Tj{.,3Bg3;?kgeY~w,"">X12</field>
                            </block>
                            </value>
                            <value name=""B"">
                            <block type=""math_number"" id=""SE(OKT/!fAEf80O1Eqwe"">
                                <field name=""NUM"">14</field>
                            </block>
                            </value>
                        </block>
                        </value>
                        <statement name=""DO2"">
                        <block type=""procedures_callnoreturn"" id=""%Wu+o$.`uwSMLD92]|.0"">
                            <mutation name=""bad guy""></mutation>
                        </block>
                        </statement>
                        <statement name=""ELSE"">
                        <block type=""variables_set"" id=""oaT.yX[lj-cVm%qq/Kg|"">
                            <field name=""VAR"" id=""Q}Tj{.,3Bg3;?kgeY~w,"">X12</field>
                            <value name=""VALUE"">
                            <block type=""math_number"" id=""V7?*[c.Yq2O^|xvPV.xG"">
                                <field name=""NUM"">2999.11263</field>
                            </block>
                            </value>
                        </block>
                        </statement>
                    </block>
                    </statement>
                </block>
            </xml>
        ";

        var engine = new ScriptEngine(Services, Services.GetRequiredService<IScriptParser>(), new NullLogger<ScriptEngine>(), null);

        await engine.EvaluateAsync(xml, []);
    }

    [TestCase(null)]
    [TestCase(30d)]
    public async Task Can_Evaluate_With_Variables_Async(double? a)
    {
        const string xml = @"
            <xml xmlns=""https://developers.google.com/blockly/xml"">
            <variables>
                <variable id=""dVAo@YOQaT_.]cmIv.]B"">a</variable>
                <variable id=""~t}_R{i-:cJsh@BfUBT7"">b</variable>
                <variable id=""#:q.*3t6B~G613m(v?Eh"">c</variable>
                <variable id=""{8YjWo|I];c|Bl|zJJyO"">result</variable>
                <variable id=""5C39O`{%+mf!{KE)C2b5"">d</variable>
            </variables>
            <block type=""variables_set"" id=""g_6TXKp%@Yck1tP7_s=b"" x=""88"" y=""55"">
                <field name=""VAR"" id=""dVAo@YOQaT_.]cmIv.]B"">a</field>
                <value name=""VALUE"">
                <block type=""math_number"" id=""4@y.iw.xaV5xMaY.uJP:"">
                    <field name=""NUM"">29</field>
                </block>
                </value>
                <next>
                <block type=""variables_set"" id=""rTS;)+ya~tV7yz}#(mR]"">
                    <field name=""VAR"" id=""~t}_R{i-:cJsh@BfUBT7"">b</field>
                    <value name=""VALUE"">
                    <block type=""math_number"" id=""fVpZ}BGgPXKLhW/zyi9q"">
                        <field name=""NUM"">9</field>
                    </block>
                    </value>
                    <next>
                    <block type=""variables_set"" id=""rrBSq6p`h{92U*6%*i1`"">
                        <field name=""VAR"" id=""#:q.*3t6B~G613m(v?Eh"">c</field>
                        <value name=""VALUE"">
                        <block type=""math_on_list"" id="")o?rf@;VsfaV,|X2tb~#"">
                            <mutation op=""SUM""></mutation>
                            <field name=""OP"">SUM</field>
                            <value name=""LIST"">
                            <block type=""lists_create_with"" id=""E%j4d_PA}@kp?_kwUYVh"">
                                <mutation items=""2""></mutation>
                                <value name=""ADD0"">
                                <block type=""variables_get"" id=""#FE2m]zVLPqFVUe8-/:t"">
                                    <field name=""VAR"" id=""dVAo@YOQaT_.]cmIv.]B"">a</field>
                                </block>
                                </value>
                                <value name=""ADD1"">
                                <block type=""variables_get"" id=""L^Yenp!P1{1aCnEKYda_"">
                                    <field name=""VAR"" id=""~t}_R{i-:cJsh@BfUBT7"">b</field>
                                </block>
                                </value>
                            </block>
                            </value>
                        </block>
                        </value>
                        <next>
                        <block type=""variables_set"" id=""7)z^aUx$37rZF)d|w71}"">
                            <field name=""VAR"" id=""{8YjWo|I];c|Bl|zJJyO"">result</field>
                            <value name=""VALUE"">
                            <block type=""lists_create_with"" id=""?vj^YpY1Z}TjuGHJ5-5E"">
                                <mutation items=""4""></mutation>
                                <value name=""ADD0"">
                                <block type=""variables_get"" id=""bGpY;rQ#T$#l!Oopp~ex"">
                                    <field name=""VAR"" id=""dVAo@YOQaT_.]cmIv.]B"">a</field>
                                </block>
                                </value>
                                <value name=""ADD1"">
                                <block type=""variables_get"" id=""xu[suKO_07B[VuITRCxC"">
                                    <field name=""VAR"" id=""~t}_R{i-:cJsh@BfUBT7"">b</field>
                                </block>
                                </value>
                                <value name=""ADD2"">
                                <block type=""variables_get"" id=""#zk-se2]zK3!5U=Y]qo?"">
                                    <field name=""VAR"" id=""#:q.*3t6B~G613m(v?Eh"">c</field>
                                </block>
                                </value>
                                <value name=""ADD3"">
                                <block type=""variables_get"" id=""IS4CTK9rh,noNL5])f+l"">
                                    <field name=""VAR"" id=""5C39O`{%+mf!{KE)C2b5"">d</field>
                                </block>
                                </value>
                            </block>
                            </value>
                        </block>
                        </next>
                    </block>
                    </next>
                </block>
                </next>
            </block>
            </xml>
        ";

        var engine = new ScriptEngine(Services, Services.GetRequiredService<IScriptParser>(), new NullLogger<ScriptEngine>(), null);

        var presets = new Dictionary<string, object?> { { "d", "Jochen" } };

        if (a.HasValue) presets["a"] = a.Value;

        var vars = (IList<object?>)(await engine.EvaluateAsync(xml, presets))!;

        Assert.Multiple(() =>
        {
            Assert.That(vars[0], Is.EqualTo(29d));
            Assert.That(vars[1], Is.EqualTo(9d));
            Assert.That(vars[2], Is.EqualTo(38d));
            Assert.That(vars[3], Is.EqualTo("Jochen"));
        });
    }

    [Test]
    public async Task Can_Execute_Generic_Http_Request_Async()
    {
        const string xml = @"
            <xml xmlns=""https://developers.google.com/blockly/xml"">
            <variables>
                <variable id=""=K5EOw-5ENmSP3Y:d5_L"">result</variable>
            </variables>
            <block type=""variables_set"" id=""%s0q,0CVkN:w)tz(3C/)"" x=""103"" y=""143"">
                <field name=""VAR"" id=""=K5EOw-5ENmSP3Y:d5_L"">result</field>
                <value name=""VALUE"">
                <block type=""http_request"" id=""$IUpskJdA,UYhJi3^tp:"">
                    <field name=""METHOD"">GET</field>
                    <field name=""URI"">Url</field>
                    <field name=""BODY"">Payload</field>
                    <field name=""AUTHMETHOD""></field>
                    <value name=""URI"">
                        <block type=""text"" id=""|]4bW:=rJR0N43C8qBm7"">
                            <field name=""TEXT"">https://www.psimarron.net</field>
                        </block>
                    </value>
                    <value name=""BODY"">
                        <block type=""text"" id=""|]4bW:=rJR0N43C8qBm8"">
                            <field name=""TEXT"">{}</field>
                        </block>
                    </value>
                </block>
                </value>
            </block>
            </xml>        
        ";

        var engine = new ScriptEngine(Services, Services.GetRequiredService<IScriptParser>(), new NullLogger<ScriptEngine>(), null);

        var body = await engine.EvaluateAsync(xml, []);

        Assert.That(body, Has.Length.GreaterThan(1000));
    }

    [Test]
    public async Task Can_Execute_Try_Catch_Finally_Async()
    {
        var xml = @"
            <xml xmlns=""https://developers.google.com/blockly/xml"">
            <variables>
                <variable id=""A9|DndqA~fHVE7M8wlvX"">a</variable>
                <variable id=""xIToC$dxYeC)_r@z7j_w"">c</variable>
                <variable id=""i)!WeW`fDKs2@D?Ca!Ml"">e</variable>
                <variable id=""4j],0^530XKfmoaA.*UE"">d</variable>
                <variable id=""YQ?t_f(Sg7^j[czNa-[G"">b</variable>
                <variable id=""fkWk2OsaAo8:3u9-58*5"">f</variable>
                <variable id=""J;UipLE5oIlOC+;SGcz8"">result</variable>
            </variables>
            <block type=""variables_set"" id="":x_K-F2]m:mzMseQ;}.Y"" x=""275"" y=""125"">
                <field name=""VAR"" id=""A9|DndqA~fHVE7M8wlvX"">a</field>
                <value name=""VALUE"">
                <block type=""math_number"" id=""QN/HKaJ%__ekOu]2*8(J"">
                    <field name=""NUM"">1</field>
                </block>
                </value>
                <next>
                <block type=""try_catch_finally"" id=""i:@^Z`7``=M$iG}2bBcl"">
                    <statement name=""TRY"">
                    <block type=""variables_set"" id=""0%{uF:*8yr2ksX]8i+S~"">
                        <field name=""VAR"" id=""xIToC$dxYeC)_r@z7j_w"">c</field>
                        <value name=""VALUE"">
                        <block type=""math_number"" id=""gi%`YJK7#PHZhPu+l?L}"">
                            <field name=""NUM"">3</field>
                        </block>
                        </value>
                        <next>
                        <block type=""variables_set"" id=""C),H$aAU^b:TM87}_~1|"">
                            <field name=""VAR"" id=""fkWk2OsaAo8:3u9-58*5"">f</field>
                        </block>
                        </next>
                    </block>
                    </statement>
                    <statement name=""CATCH"">
                    <block type=""variables_set"" id=""{zF3C:)DbypuGvi6L]HA"">
                        <field name=""VAR"" id=""i)!WeW`fDKs2@D?Ca!Ml"">e</field>
                        <value name=""VALUE"">
                        <block type=""math_number"" id=""$#rJ?WMAgjSs]f~y~n{)"">
                            <field name=""NUM"">5</field>
                        </block>
                        </value>
                    </block>
                    </statement>
                    <statement name=""FINALLY"">
                    <block type=""variables_set"" id=""lBjGakE7A3@AA#g2aKC%"">
                        <field name=""VAR"" id=""4j],0^530XKfmoaA.*UE"">d</field>
                        <value name=""VALUE"">
                        <block type=""math_number"" id=""`39y?HFVE-GV{WbPHlRv"">
                            <field name=""NUM"">4</field>
                        </block>
                        </value>
                    </block>
                    </statement>
                    <next>
                    <block type=""variables_set"" id=""9C(ItX,%yA/ULg`{O{Z."">
                        <field name=""VAR"" id=""YQ?t_f(Sg7^j[czNa-[G"">b</field>
                        <value name=""VALUE"">
                        <block type=""math_number"" id=""H)xpcn{oO}KXZ#-qLh=b"">
                            <field name=""NUM"">2</field>
                        </block>
                        </value>
                        <next>
                        <block type=""variables_set"" id=""x~ENROl}LVrIb4t8/z{)"">
                            <field name=""VAR"" id=""J;UipLE5oIlOC+;SGcz8"">result</field>
                            <value name=""VALUE"">
                            <block type=""lists_create_with"" id=""+086sR(c,8zTzrQqDxaN"">
                                <mutation items=""6""></mutation>
                                <value name=""ADD0"">
                                <block type=""variables_get"" id=""XlOT[qOKB@cqeXf!N0vF"">
                                    <field name=""VAR"" id=""A9|DndqA~fHVE7M8wlvX"">a</field>
                                </block>
                                </value>
                                <value name=""ADD1"">
                                <block type=""variables_get"" id="")xt/|yDZ4:Ong[{=THpr"">
                                    <field name=""VAR"" id=""YQ?t_f(Sg7^j[czNa-[G"">b</field>
                                </block>
                                </value>
                                <value name=""ADD2"">
                                <block type=""variables_get"" id=""eRceD,,M6P~l$Y}8ZZN~"">
                                    <field name=""VAR"" id=""xIToC$dxYeC)_r@z7j_w"">c</field>
                                </block>
                                </value>
                                <value name=""ADD3"">
                                <block type=""variables_get"" id=""4LEh4G?0pAaSx^EEzGRg"">
                                    <field name=""VAR"" id=""4j],0^530XKfmoaA.*UE"">d</field>
                                </block>
                                </value>
                                <value name=""ADD4"">
                                <block type=""variables_get"" id=""iTZTJ?dot@!(2BAoc?rY"">
                                    <field name=""VAR"" id=""i)!WeW`fDKs2@D?Ca!Ml"">e</field>
                                </block>
                                </value>
                                <value name=""ADD5"">
                                <block type=""variables_get"" id=""?,Y8#qB3EqEkD_FrM|{4"">
                                    <field name=""VAR"" id=""fkWk2OsaAo8:3u9-58*5"">f</field>
                                </block>
                                </value>
                            </block>
                            </value>
                        </block>
                        </next>
                    </block>
                    </next>
                </block>
                </next>
            </block>
            </xml>
        ";

        var engine = new ScriptEngine(Services, Services.GetRequiredService<IScriptParser>(), new NullLogger<ScriptEngine>(), null);

        var vars = (IList<object>)(await engine.EvaluateAsync(xml, []))!;

        Assert.Multiple(() =>
        {
            Assert.That(vars[0], Is.EqualTo(1));
            Assert.That(vars[1], Is.EqualTo(2));
            Assert.That(vars[2], Is.EqualTo(3));
            Assert.That(vars[3], Is.EqualTo(4));
            Assert.That(vars[4], Is.EqualTo(5));
            Assert.That(vars[5], Is.Null);
        });
    }
}