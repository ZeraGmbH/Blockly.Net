using BlocklyNet.Scripting.Engine;
using BlocklyNet.Scripting.Generic;
using NUnit.Framework;

namespace BlocklyNetTests.Engine;

[TestFixture]
public class CancellationTests : TestEnvironment
{
    /* A bad script running forever and just counting a variable. */
    private const string EndLessLoop = @"
        <xml xmlns=""https://developers.google.com/blockly/xml"">
        <variables>
            <variable id=""!dW.xvOMUapXjlDKv2=1"">result</variable>
        </variables>
        <block type=""variables_set"" id=""M?h}S41(`5W%r$dcC!KH"" x=""225"" y=""75"">
            <field name=""VAR"" id=""!dW.xvOMUapXjlDKv2=1"">result</field>
            <value name=""VALUE"">
            <block type=""math_number"" id=""`rBd$0.YSukD(u?q*dLk"">
                <field name=""NUM"">0</field>
            </block>
            </value>
            <next>
            <block type=""controls_whileUntil"" id=""xi#s}84jVAG^|vEP3#?Q"">
                <field name=""MODE"">WHILE</field>
                <value name=""BOOL"">
                <block type=""logic_boolean"" id=""a0;GE{iqfyezW_^6-hod"">
                    <field name=""BOOL"">TRUE</field>
                </block>
                </value>
                <statement name=""DO"">
                <block type=""variables_set"" id=""?A0,8?wp-s)586s-CKKO"">
                    <field name=""VAR"" id=""!dW.xvOMUapXjlDKv2=1"">result</field>
                    <value name=""VALUE"">
                    <block type=""math_arithmetic"" id=""Vz0FYR~~tst#C!$YW!Yt"">
                        <field name=""OP"">ADD</field>
                        <value name=""A"">
                        <shadow type=""math_number"" id=""y;_A?yc(.1Cg},`Fr@j_"">
                            <field name=""NUM"">1</field>
                        </shadow>
                        <block type=""variables_get"" id=""l65(eH3c7y]]swdu/cRz"">
                            <field name=""VAR"" id=""!dW.xvOMUapXjlDKv2=1"">result</field>
                        </block>
                        </value>
                        <value name=""B"">
                        <shadow type=""math_number"" id=""|*X:./hdh4Q,,V_Qrb`_"">
                            <field name=""NUM"">1</field>
                        </shadow>
                        </value>
                    </block>
                    </value>
                </block>
                </statement>
            </block>
            </next>
        </block>
    </xml>";

    /// <summary>
    /// See if we can do a hard cancel of the endless loop.
    /// </summary>
    [Test]
    public void Can_Cancel_A_Running_Script()
    {
        /* Prepare to terminate the loop after 0.5 seconds. */
        Cancel.CancelAfter(500);

        var script = Engine.Parser.Parse(EndLessLoop);

        Assert.ThrowsAsync<OperationCanceledException>(async () => await script.RunAsync(Site.Object));
    }

    /// <summary>
    /// See if we can do a soft cancel of the endless loop. Here
    /// the loop will terminate like on a hard cancel but the
    /// result calculated so far can still be kept.
    /// </summary>
    [Test]
    public async Task Can_Silently_Cancel_A_Running_Script_Async()
    {
        /* Prepare to cancel after 0.5 seconds. */
        var cancel = new CancellationTokenSource();

        cancel.CancelAfter(500);

        /* Execute the no longer so end-less loop. */
        var result = await ((IScriptSite)Engine).RunAsync<GenericResult, StartGenericScript>(
            new() { Name = "Will be stopped", ScriptId = AddScript("SCRIPT", EndLessLoop) },
            new() { ShouldStopNow = () => cancel.IsCancellationRequested }
        );

        /* Should have done anything. */
        Assert.That(result.Result, Is.GreaterThan(0));
    }
}