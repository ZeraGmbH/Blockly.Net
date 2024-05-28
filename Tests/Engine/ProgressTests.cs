using System.Collections;
using BlocklyNet.Scripting.Engine;
using BlocklyNet.Scripting.Generic;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BlocklyNetTests.Engine;

[TestFixture]
public class ProgressTests : TestEnvironment
{
    /* Script generating some progress information and reporting time stamps after delays. */
    private const string script = @"
        <xml xmlns=""https://developers.google.com/blockly/xml"">
        <variables>
            <variable id=""!dW.xvOMUapXjlDKv2=1"">result</variable>
        </variables>
        <block type=""variables_set"" id=""xLJ{]2Yi7g5|sK#WfC!A"" x=""275"" y=""-25"">
            <field name=""VAR"" id=""!dW.xvOMUapXjlDKv2=1"">result</field>
            <value name=""VALUE"">
            <block type=""lists_create_with"" id=""njGCV[qN1zlk=n=J+ZI1"">
                <mutation items=""1""></mutation>
                <value name=""ADD0"">
                <block type=""get_date_time"" id=""C*|OVg}f:nA5sZy]~8I%"">
                    <field name=""FORMAT"">Format</field>
                    <value name=""FORMAT"">
                    <shadow type=""text"" id=""a}8VgpISa(jFdN?O/8gh"">
                        <field name=""TEXT"">dd.MM.yyyy HH:mm:ss</field>
                    </shadow>
                    <block type=""logic_null"" id=""(kqY*oTOii@^=Zucoq(#""></block>
                    </value>
                </block>
                </value>
            </block>
            </value>
            <next>
            <block type=""set_progress"" id=""7G^~Q4EsWQGC[AO8.o+_"">
                <field name=""NAME"">Name of the progress</field>
                <field name=""PROGRESS"">Progress (%)</field>
                <field name=""PAYLOAD"">Extra data</field>
                <field name=""PAYLOADTYPE"">Type of extra data</field>
                <value name=""NAME"">
                <shadow type=""text"" id=""||QXBI$CP3s.,kw:fR@5"">
                    <field name=""TEXT"">Script</field>
                </shadow>
                </value>
                <value name=""PROGRESS"">
                <shadow type=""math_number"" id=""W3!*SOQ8p3wYt.;MN1?u"">
                    <field name=""NUM"">0</field>
                </shadow>
                </value>
                <value name=""PAYLOADTYPE"">
                <shadow type=""text"" id=""t%ycIMht)rNxpicd9hUV"">
                    <field name=""TEXT""></field>
                </shadow>
                </value>
                <next>
                <block type=""delay"" id=""G~kqCCue8$cR98ajs|j+"">
                    <field name=""DELAY"">Delay (ms)</field>
                    <value name=""DELAY"">
                    <shadow type=""math_number"" id=""t0jN!TdX3;jD+QRSd-Gm"">
                        <field name=""NUM"">100</field>
                    </shadow>
                    </value>
                    <next>
                    <block type=""lists_setIndex"" id=""*VqPt/V!H^4V)EW2hy|B"">
                        <mutation at=""true""></mutation>
                        <field name=""MODE"">INSERT</field>
                        <field name=""WHERE"">FROM_END</field>
                        <value name=""LIST"">
                        <block type=""variables_get"" id=""%NpTRH[A8J:|}W8g%oeg"">
                            <field name=""VAR"" id=""!dW.xvOMUapXjlDKv2=1"">result</field>
                        </block>
                        </value>
                        <value name=""AT"">
                        <block type=""math_number"" id=""MY{F(|dw9t2ZOo`-fYAf"">
                            <field name=""NUM"">0</field>
                        </block>
                        </value>
                        <value name=""TO"">
                        <block type=""get_date_time"" id=""[[0Fa[PbOOdo}@lHJvQ~"">
                            <field name=""FORMAT"">Format</field>
                            <value name=""FORMAT"">
                            <shadow type=""text"" id=""a}8VgpISa(jFdN?O/8gh"">
                                <field name=""TEXT"">dd.MM.yyyy HH:mm:ss</field>
                            </shadow>
                            <block type=""logic_null"" id=""_SAi%43r=`3+/i+(ilXz""></block>
                            </value>
                        </block>
                        </value>
                        <next>
                        <block type=""set_progress"" id=""HtUFT{h?Zx~CoBkY^[kc"">
                            <field name=""NAME"">Name of the progress</field>
                            <field name=""PROGRESS"">Progress (%)</field>
                            <field name=""PAYLOAD"">Extra data</field>
                            <field name=""PAYLOADTYPE"">Type of extra data</field>
                            <value name=""NAME"">
                            <shadow type=""text"" id=""*0/P$5F75k,+r3,8zc[P"">
                                <field name=""TEXT"">Script</field>
                            </shadow>
                            </value>
                            <value name=""PROGRESS"">
                            <shadow type=""math_number"" id=""jyT!27r77yuv6!Uv{Hh("">
                                <field name=""NUM"">33</field>
                            </shadow>
                            </value>
                            <value name=""PAYLOADTYPE"">
                            <shadow type=""text"" id=""6:BYCl%`?I)C_`(w_$Wb"">
                                <field name=""TEXT""></field>
                            </shadow>
                            </value>
                            <next>
                            <block type=""delay"" id=""J[8P/:4+INZd}bW/^B%`"">
                                <field name=""DELAY"">Delay (ms)</field>
                                <value name=""DELAY"">
                                <shadow type=""math_number"" id=""5_D@z4L{HiG|,uAjw){|"">
                                    <field name=""NUM"">150</field>
                                </shadow>
                                </value>
                                <next>
                                <block type=""lists_setIndex"" id="";zFUtWznC0|CREz:-hPO"">
                                    <mutation at=""true""></mutation>
                                    <field name=""MODE"">INSERT</field>
                                    <field name=""WHERE"">FROM_END</field>
                                    <value name=""LIST"">
                                    <block type=""variables_get"" id=""{p%C2,qr_6(z@TkZ@98s"">
                                        <field name=""VAR"" id=""!dW.xvOMUapXjlDKv2=1"">result</field>
                                    </block>
                                    </value>
                                    <value name=""AT"">
                                    <block type=""math_number"" id=""[OdG:xdj-]0I)y~f?p55"">
                                        <field name=""NUM"">0</field>
                                    </block>
                                    </value>
                                    <value name=""TO"">
                                    <block type=""get_date_time"" id=""YdH|O})V](SYno(Y)@=Q"">
                                        <field name=""FORMAT"">Format</field>
                                        <value name=""FORMAT"">
                                        <shadow type=""text"" id=""a}8VgpISa(jFdN?O/8gh"">
                                            <field name=""TEXT"">dd.MM.yyyy HH:mm:ss</field>
                                        </shadow>
                                        <block type=""logic_null"" id=""yR5f%dVUrqgY3yFPLBPv""></block>
                                        </value>
                                    </block>
                                    </value>
                                    <next>
                                    <block type=""set_progress"" id=""S^uFYp.6TDEsnyp]V^w|"">
                                        <field name=""NAME"">Name of the progress</field>
                                        <field name=""PROGRESS"">Progress (%)</field>
                                        <field name=""PAYLOAD"">Extra data</field>
                                        <field name=""PAYLOADTYPE"">Type of extra data</field>
                                        <value name=""NAME"">
                                        <shadow type=""text"" id=""XWg|%Pi$d=}|Fa`0r4pa"">
                                            <field name=""TEXT"">Script</field>
                                        </shadow>
                                        </value>
                                        <value name=""PROGRESS"">
                                        <shadow type=""math_number"" id=""FI`s-kjw8/ov[0M4BzR,"">
                                            <field name=""NUM"">100</field>
                                        </shadow>
                                        </value>
                                        <value name=""PAYLOADTYPE"">
                                        <shadow type=""text"" id=""Msvg77Q-bZ{jrhT9:|=P"">
                                            <field name=""TEXT""></field>
                                        </shadow>
                                        </value>
                                    </block>
                                    </next>
                                </block>
                                </next>
                            </block>
                            </next>
                        </block>
                        </next>
                    </block>
                    </next>
                </block>
                </next>
            </block>
            </next>
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
    /// Check if the engine will provide progress information.
    /// </summary>
    [Test]
    public async Task Can_Provide_Progress()
    {
        /* Termination helper. */
        var done = new TaskCompletionSource();

        /* Configure the broadcast sink. */
        var progress = 0;

        ((Sink)GetService<IScriptEngineNotifySink>()).OnEvent = (method, arg) =>
        {
            switch (progress++)
            {
                case 0:
                    /* The script has been started. */
                    Assert.That(method, Is.EqualTo(ScriptEngineNotifyMethods.Started));
                    Assert.That(arg, Is.TypeOf<ScriptInformation>());
                    break;
                case 1:
                case 2:
                case 3:
                    /* The SetProgress block has been executed. */
                    Assert.That(method, Is.EqualTo(ScriptEngineNotifyMethods.Progress));
                    Assert.That(arg, Is.TypeOf<ScriptProgress>());
                    Assert.That(((ScriptProgress)arg!).AllProgress[0].Progress, Is.EqualTo(progress == 2 ? 0 : progress == 3 ? 0.33d : 1));
                    break;
                case 4:
                    /* blockly finished the script. */
                    done.SetResult();

                    Assert.That(method, Is.EqualTo(ScriptEngineNotifyMethods.Done));
                    Assert.That(arg, Is.TypeOf<ScriptDone>());
                    break;
                case 5:
                    /* The result of the script has been requested. */
                    Assert.That(method, Is.EqualTo(ScriptEngineNotifyMethods.Finished));
                    Assert.That(arg, Is.TypeOf<ScriptFinished>());
                    break;
                default:
                    Assert.Fail("too may events");
                    break;
            }
        };

        var jobId = Engine.Start(new StartGenericScript { Name = "Will generate progress", ScriptId = AddScript("SCRIPT", script) }, "");

        /* Wait for the script to finish. */
        await done.Task;

        /* Check the time stamps generated. */
        var result = (GenericResult)Engine.FinishScriptAndGetResult(jobId)!;
        var times = ((IEnumerable)result.Result).Cast<double>().Select(d => new DateTime((long)d)).ToArray();

        Assert.That((times[1] - times[0]).TotalMilliseconds, Is.GreaterThanOrEqualTo(100));
        Assert.That((times[2] - times[1]).TotalMilliseconds, Is.GreaterThanOrEqualTo(140));

        /* Check for correct number of events. */
        Assert.That(progress, Is.EqualTo(6));
    }
}