using System.Collections;
using BlocklyNet.Scripting.Engine;
using BlocklyNet.Scripting.Generic;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BlocklyNetTests.Engine;

[TestFixture]
public class ParallelTests : TestEnvironment
{
    /* Script to be run in parallel - just waits a bit and reports run-time. */
    private const string single = @"
    <xml xmlns=""https://developers.google.com/blockly/xml"">
        <variables>
            <variable id=""!dW.xvOMUapXjlDKv2=1"">result</variable>
            <variable id=""RVFNNba_=-ceiC+=#8tC"">delay</variable>
        </variables>
        <block type=""variables_set"" id=""/gGpDUbaf3aSq5$-im~a"" x=""225"" y=""125"">
            <field name=""VAR"" id=""!dW.xvOMUapXjlDKv2=1"">result</field>
            <value name=""VALUE"">
            <block type=""lists_create_with"" id=""]5@orE34LJm./ykxzEl*"">
                <mutation items=""1""></mutation>
                <value name=""ADD0"">
                <block type=""get_date_time"" id=""~F)Z`dpzAdB^OO`Zy4eF"">
                    <field name=""FORMAT"">Format</field>
                    <value name=""FORMAT"">
                    <shadow type=""text"" id=""oV98FCm(|06tTlia_aDG"">
                        <field name=""TEXT"">dd.MM.yyyy HH:mm:ss</field>
                    </shadow>
                    <block type=""logic_null"" id=""M;[-Kyt6x7I!IMF$B]6$""></block>
                    </value>
                </block>
                </value>
            </block>
            </value>
            <next>
            <block type=""delay"" id="",e,$[.F0I%,forMs5DGc"">
                <field name=""DELAY"">Delay (ms)</field>
                <value name=""DELAY"">
                <shadow type=""math_number"" id=""e*/0QNZK):^;EkxDupk?"">
                    <field name=""NUM"">0</field>
                </shadow>
                <block type=""variables_get"" id=""_=kWaa`B8SiBte`z5U^)"">
                    <field name=""VAR"" id=""RVFNNba_=-ceiC+=#8tC"">delay</field>
                </block>
                </value>
                <next>
                <block type=""lists_setIndex"" id=""*Ot:xGFS]F9r.-H+KL^~"">
                    <mutation at=""true""></mutation>
                    <field name=""MODE"">INSERT</field>
                    <field name=""WHERE"">FROM_END</field>
                    <value name=""LIST"">
                    <block type=""variables_get"" id=""oV}LI%w1-T3~)3+e8NU$"">
                        <field name=""VAR"" id=""!dW.xvOMUapXjlDKv2=1"">result</field>
                    </block>
                    </value>
                    <value name=""AT"">
                    <block type=""math_number"" id=""ZnE`zcl~u1YV~H[~uA1g"">
                        <field name=""NUM"">0</field>
                    </block>
                    </value>
                    <value name=""TO"">
                    <block type=""get_date_time"" id=""%jkq]zRBS(o0dgyxH-11"">
                        <field name=""FORMAT"">Format</field>
                        <value name=""FORMAT"">
                        <shadow type=""text"" id=""oV98FCm(|06tTlia_aDG"">
                            <field name=""TEXT"">dd.MM.yyyy HH:mm:ss</field>
                        </shadow>
                        <block type=""logic_null"" id=""$ViE2q0(|H{gDKGB~^$)""></block>
                        </value>
                    </block>
                    </value>
                </block>
                </next>
            </block>
            </next>
        </block>
    </xml>";

    /* Script starting two parallel executions, */
    private string parallel = @"
    <xml xmlns=""https://developers.google.com/blockly/xml"">
    <variables>
        <variable id=""!dW.xvOMUapXjlDKv2=1"">result</variable>
    </variables>
    <block type=""variables_set"" id=""/gGpDUbaf3aSq5$-im~a"" x=""225"" y=""125"">
        <field name=""VAR"" id=""!dW.xvOMUapXjlDKv2=1"">result</field>
        <value name=""VALUE"">
        <block type=""run_script_in_parallel"" id=""g+xk==,qmGNhtylWJ:jz"">
            <field name=""SCRIPTS"">scripts</field>
            <field name=""LEADINGSCRIPT"">leading</field>
            <value name=""SCRIPTS"">
            <block type=""lists_create_with"" id=""DQ]hEfBF[w:nj4et[tD$"">
                <mutation items=""2""></mutation>
                <value name=""ADD0"">
                <block type=""run_script_by_name"" id=""/(2y{xH?:SRSk(ML*QQ-"">
                    <field name=""NAME"">Display name</field>
                    <field name=""ARGS"">Parameters</field>
                    <value name=""NAME"">
                    <shadow type=""text"" id=""/bG6]YgQ/Tei](Nq@4H`"">
                        <field name=""TEXT"">SINGLE</field>
                    </shadow>
                    </value>
                    <value name=""ARGS"">
                    <block type=""lists_create_with"" id=""LsNN6]JBYp;v([#eB~j6"">
                        <mutation items=""1""></mutation>
                        <value name=""ADD0"">
                        <block type=""create_script_parameter"" id=""=d.lw=nOtR3W]9LAR(Ay"">
                            <field name=""NAME"">Variable name</field>
                            <field name=""VALUE"">Value</field>
                            <value name=""NAME"">
                            <shadow type=""text"" id=""?S=Y6UZx|(:?3bue!d1{"">
                                <field name=""TEXT"">delay</field>
                            </shadow>
                            </value>
                            <value name=""VALUE"">
                            <block type=""math_number"" id=""ee,9s_a-=@wy@BW:Cyjm"">
                                <field name=""NUM"">500</field>
                            </block>
                            </value>
                        </block>
                        </value>
                    </block>
                    </value>
                </block>
                </value>
                <value name=""ADD1"">
                <block type=""run_script_by_name"" id=""D-vqi=GU;Mp69@GHMkQ|"">
                    <field name=""NAME"">Display name</field>
                    <field name=""ARGS"">Parameters</field>
                    <value name=""NAME"">
                    <shadow type=""text"" id=""HccU(*]FRH_K..5*DzWV"">
                        <field name=""TEXT"">SINGLE</field>
                    </shadow>
                    </value>
                    <value name=""ARGS"">
                    <block type=""lists_create_with"" id=""})LI]ik24YcbQ,f)SlF0"">
                        <mutation items=""1""></mutation>
                        <value name=""ADD0"">
                        <block type=""create_script_parameter"" id=""3$r}A.be2SH+)U]HYi}}"">
                            <field name=""NAME"">Variable name</field>
                            <field name=""VALUE"">Value</field>
                            <value name=""NAME"">
                            <shadow type=""text"" id=""SIzs:5$%TmWRz;S(49Ie"">
                                <field name=""TEXT"">delay</field>
                            </shadow>
                            </value>
                            <value name=""VALUE"">
                            <block type=""math_number"" id=""l@rOa@MYbdL#$?xn2#zz"">
                                <field name=""NUM"">750</field>
                            </block>
                            </value>
                        </block>
                        </value>
                    </block>
                    </value>
                </block>
                </value>
            </block>
            </value>
        </block>
        </value>
    </block>
    </xml>";

    /// <inheritdoc/>
    protected override void OnSetup(IServiceCollection services)
    {
        base.OnSetup(services);

        /* Register the broadcast sink. */
        services.AddSingleton<IScriptEngineNotifySink, Sink>();
    }

    /// <summary>
    /// Run two scripts in parallel to each other.
    /// </summary>
    [Test]
    public async Task Can_Run_In_Parallel()
    {
        /* Termination helper. */
        var done = new TaskCompletionSource();

        /* Configure the broadcast sink. */
        ((Sink)GetService<IScriptEngineNotifySink>()).OnEvent = (method, arg) => { if (method == ScriptEngineNotifyMethods.Done) done.SetResult(); };

        AddScript("SINGLE", single, [new() { Name = "delay", Type = "number", Required = true }]);

        var jobId = Engine.Start(new StartGenericScript { Name = "Will run in parallel", ScriptId = AddScript("MULTI", parallel) }, "");

        /* Wait for the script to finish. */
        await done.Task;

        /* Check the time stamps generated. */
        var result = (GenericResult)Engine.FinishScriptAndGetResult(jobId)!;
        var results = ((IEnumerable)result.Result).Cast<IEnumerable>().ToArray();
        var times1 = results[0].Cast<double>().Select(t => new DateTime((long)t)).ToArray();
        var times2 = results[1].Cast<double>().Select(t => new DateTime((long)t)).ToArray();

        Assert.That((times1[1] - times1[0]).TotalMilliseconds, Is.GreaterThan(490));
        Assert.That((times2[1] - times2[0]).TotalMilliseconds, Is.GreaterThan(740));

        Assert.That(Math.Abs((times1[0] - times2[0]).TotalMilliseconds), Is.LessThan(250));
    }
}