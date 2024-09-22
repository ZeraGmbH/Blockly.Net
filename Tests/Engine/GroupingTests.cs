using System.Text.Json;
using BlocklyNet;
using BlocklyNet.Scripting.Engine;
using BlocklyNet.Scripting.Generic;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BlocklyNetTests.Engine;

[TestFixture]
public class GroupingTests : TestEnvironment
{
    /* Just request some user input. */
    private const string Script1 = @"
        <xml xmlns=""https://developers.google.com/blockly/xml"">
        <block type=""execute_group"" id=""Outer"">
            <value name=""RESULT"">                
                <block type=""group_execution_result"" id=""Outer.Result"">
                    <value name=""Result"">                
                        <block type=""text"" id=""Outer.Result.Value"">
                            <field name=""TEXT"">1</field>
                        </block>
                    </value>
                    <value name=""Type"">                
                        <block type=""group_execution_result_type"" id=""Outer.Result.Value.Type"">
                            <field name=""VALUE"">Succeeded</field>
                        </block>
                    </value>
                </block>
            </value>
            <next>
                <block type=""execute_group"" id=""Inner"">
                    <value name=""RESULT"">                
                        <block type=""group_execution_result"" id=""Inner.Result"">
                            <value name=""Result"">                
                                <block type=""text"" id=""Inner.Result.Value"">
                                    <field name=""TEXT"">2</field>
                                </block>
                            </value>
                            <value name=""Type"">                
                                <block type=""group_execution_result_type"" id=""Inner.Result.Value.Type"">
                                    <field name=""VALUE"">Failed</field>
                                </block>
                            </value>
                        </block>
                    </value>
                </block>
            </next>
        </block>
        </xml>";

    private const string Script2 = @"
        <xml xmlns=""https://developers.google.com/blockly/xml"">
        <block type=""procedures_defreturn"" id=""V_XO,cjG??A#2*p!kMlu"" x=""875"" y=""75"">
            <field name=""NAME"">Wait a bit</field>
            <comment pinned=""false"" h=""80"" w=""160"">Describe this function...</comment>
            <statement name=""STACK"">
            <block type=""delay"" id=""iD_m(VVb=YIY3A~u2(sy"">
                <field name=""DELAY"">Delay (ms)</field>
                <value name=""DELAY"">
                <shadow type=""math_number"" id=""o+pgs:6XrzMPHiK5l!@!"">
                    <field name=""NUM"">500</field>
                </shadow>
                </value>
            </block>
            </statement>
            <value name=""RETURN"">
            <block type=""math_number"" id=""7[bqSm*qUh)_I{D*IlsV"">
                <field name=""NUM"">2</field>
            </block>
            </value>
        </block>
        <block type=""execute_group"" id=""X~V-HnS3^/Kt4H)%BuoZ"" x=""175"" y=""125"">
            <field name=""NAME"">Name of the group</field>
            <field name=""RESULT"">Result</field>
            <value name=""NAME"">
            <shadow type=""text"" id="")mP+6-^uv8_i),OEqe9D"">
                <field name=""TEXT"">One</field>
            </shadow>
            </value>
            <value name=""RESULT"">
            <block type=""group_execution_result"" id=""/g~|rqb5t7yYK*.98O0l"">
                <field name=""Type"">Type</field>
                <field name=""Result"">Result</field>
                <value name=""Type"">
                <shadow type=""group_execution_result_type"" id=""w,B.`+|YSD=n)bAqpi4O"">
                    <field name=""VALUE"">Succeeded</field>
                </shadow>
                </value>
                <value name=""Result"">
                <block type=""math_number"" id="")2Bi0@WsT_gr.P{AK=Nw"">
                    <field name=""NUM"">1</field>
                </block>
                </value>
            </block>
            </value>
            <next>
            <block type=""execute_group"" id=""30_ieh}RB`Iwu:eV,AFx"">
                <field name=""NAME"">Name of the group</field>
                <field name=""RESULT"">Result</field>
                <value name=""NAME"">
                <shadow type=""text"" id=""LnuH/Yav4A{vUQ^_SQwq"">
                    <field name=""TEXT"">Two</field>
                </shadow>
                </value>
                <value name=""RESULT"">
                <block type=""group_execution_result"" id=""H%`$2;N6A@6aQulC[jwC"">
                    <field name=""Type"">Type</field>
                    <field name=""Result"">Result</field>
                    <value name=""Type"">
                    <shadow type=""group_execution_result_type"" id=""j#9-aQR_G+rVEo[(owM)"">
                        <field name=""VALUE"">Failed</field>
                    </shadow>
                    </value>
                    <value name=""Result"">
                    <block type=""procedures_callreturn"" id=""H`IQ7RN3J6?9qI=NBom%"">
                        <mutation name=""Wait a bit""></mutation>
                    </block>
                    </value>
                </block>
                </value>
                <next>
                <block type=""execute_group"" id=""+DafGPJZI0?7`j%,9a/1"">
                    <field name=""NAME"">Name of the group</field>
                    <field name=""RESULT"">Result</field>
                    <value name=""NAME"">
                    <shadow type=""text"" id=""X?anNKQpX0Do1mszr1!6"">
                        <field name=""TEXT"">Three</field>
                    </shadow>
                    </value>
                    <value name=""RESULT"">
                    <block type=""group_execution_result"" id=""6w,g%%98{3QEFOD@,dWw"">
                        <field name=""Type"">Type</field>
                        <field name=""Result"">Result</field>
                        <value name=""Type"">
                        <shadow type=""group_execution_result_type"" id=""T2z*fcFPj,P!Skd`vbg`"">
                            <field name=""VALUE"">Succeeded</field>
                        </shadow>
                        </value>
                        <value name=""Result"">
                        <block type=""math_number"" id=""KBpA_5da#H+I^CJS{_O/"">
                            <field name=""NUM"">3</field>
                        </block>
                        </value>
                    </block>
                    </value>
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

    [Test]
    public async Task Can_Run_Groups_In_Sequence_Async()
    {
        /* Termination helper. */
        var done = new TaskCompletionSource();

        ((Sink)GetService<IScriptEngineNotifySink>()).OnEvent = (method, arg) =>
        {
            /* See if script is done. */
            if (method == ScriptEngineNotifyMethods.Done)
                done.SetResult();
            else if (method == ScriptEngineNotifyMethods.Error)
                done.SetResult();
        };

        var jobId = await Engine.StartAsync(new StartGenericScript { Name = "Run Groups", ScriptId = AddScript("SCRIPT", Script1) }, "");

        /* Wait for the script to finish. */
        await done.Task;

        /* Check the result. */
        var result = (GenericResult)Engine.FinishScriptAndGetResult(jobId)!;

        Assert.That((IList<object?>)result.Result, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task Can_Retrieve_Hash_Of_Script_Async()
    {
        /* Termination helper. */
        var done = new TaskCompletionSource();

        HashSet<string> hash = [];
        ScriptGroupStatus status = null!;

        ((Sink)GetService<IScriptEngineNotifySink>()).OnEvent = (method, arg) =>
        {
            /* See if script is done. */
            if (method != ScriptEngineNotifyMethods.Error && method != ScriptEngineNotifyMethods.Done)
                return;

            var info = (ScriptInformationWithGroupStatus)arg!;

            status = info.GroupStatus;
            hash.Add(status.CodeHash);

            done.SetResult();
        };

        var jobId = await Engine.StartAsync(new StartGenericScript { Name = "Run Groups", ScriptId = AddScript("SCRIPT", Script1) }, "");

        /* Wait for the script to finish. */
        await done.Task;

        /* Validate hash. */
        Assert.That(hash.Single(), Is.EqualTo("E4-B0-3E-B6-CF-2E-10-C5-33-C5-C4-E7-DF-CA-2A-8D-84-3D-E7-AE-BC-5D-98-E5-A3-9E-FC-F4-C1-97-3B-B5"));

        /* Validate status. */
        Assert.That(status, Is.Not.Null);
        Assert.That(status.GroupStatus, Has.Count.EqualTo(2));

        Assert.Multiple(() =>
        {
            Assert.That(status.GroupStatus[0].Key, Is.EqualTo("Outer"));
            Assert.That(status.GroupStatus[1].Key, Is.EqualTo("Inner"));
        });

        /* Check the result. */
        var result = (GenericResult)Engine.FinishScriptAndGetResult(jobId)!;

        Assert.That((IList<object?>)result.Result, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task Can_Pause_Script_Async()
    {
        /* Termination helper. */
        var done = new TaskCompletionSource();

        ScriptError? errorEvent = null;

        ((Sink)GetService<IScriptEngineNotifySink>()).OnEvent = (method, arg) =>
        {
            /* See if script is done. */
            if (method != ScriptEngineNotifyMethods.Done)
                if (method == ScriptEngineNotifyMethods.Error)
                    errorEvent = (ScriptError?)arg;
                else
                    return;

            done.SetResult();
        };

        var jobId = await Engine.StartAsync(new StartGenericScript { Name = "Run Groups", ScriptId = AddScript("SCRIPT", Script2) }, "");

        /* Pause the operation. */
        Engine.Pause(jobId);

        /* Wait for the script to finish. */
        await done.Task;

        /* Check the result. */
        var result = (GenericResult)Engine.FinishScriptAndGetResult(jobId)!;

        Assert.That(result, Is.Null);
        Assert.That(errorEvent, Is.Not.Null);

        var groups = errorEvent.GroupStatus.GroupStatus;

        Assert.That(groups, Has.Count.EqualTo(1));

        var hash = errorEvent.GroupStatus.CodeHash;

        var repeat = GroupManagerTests.MakeRepeat(groups[0], GroupRepeatType.Skip);

        repeat.SetResult(new() { Type = GroupResultType.Succeeded, Result = 42 });

        errorEvent = null;

        done = new TaskCompletionSource();

        jobId = await Engine.StartAsync(
            new StartGenericScript { Name = "Run Groups", ScriptId = AddScript("SCRIPT", Script2) },
            "",
            new() { GroupResults = new() { CodeHash = hash, GroupStatus = { repeat } } }
        );

        await done.Task;

        result = (GenericResult)Engine.FinishScriptAndGetResult(jobId)!;

        Assert.That(errorEvent, Is.Null);

        var list = (List<object?>)result.Result;

        Assert.That(list, Has.Count.EqualTo(3));
        Assert.That(((JsonElement)list[0]!).ToJsonScalar(), Is.EqualTo(42));
    }
}