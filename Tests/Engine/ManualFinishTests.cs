using BlocklyNet.Scripting.Engine;
using BlocklyNet.Scripting.Generic;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BlocklyNetTests.Engine;

[TestFixture]
public class ManualFinishTests : TestEnvironment
{
    private const string Script1 = @"
        <xml xmlns=""https://developers.google.com/blockly/xml"">
        <variables>
            <variable>result</variable>
        </variables>
        <block type=""variables_set"">
            <field name=""VAR"">result</field>
            <value name=""VALUE"">
                <block type=""text"">
                    <field name=""TEXT"">READY</field>
                </block>
            </value>
        </block>
        </xml>";

    private const string Script2 = @"
        <xml xmlns=""https://developers.google.com/blockly/xml"">
        <variables>
            <variable>result</variable>
        </variables>
        <block type=""variables_set"">
            <field name=""VAR"">result</field>
            <value name=""VALUE"">
                <block type=""text"">
                    <field name=""TEXT"">READY</field>
                </block>
            </value>
            <next>
                <block type=""throw_exception"">
                    <value name=""MESSAGE"">
                        <block type=""text"">
                            <field name=""TEXT"">AUTSCH</field>
                        </block>
                    </value>
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
    public async Task Can_Request_Result_Multiple_Times_Async()
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

        var jobId = await Engine.StartAsync(new StartGenericScript { Name = "Constant String", ScriptId = AddScript("SCRIPT", Script1) }, "");

        /* Wait for the script to finish. */
        await done.Task;

        /* Check the result. */
        for (var i = 10; i-- > 0;)
        {
            var result = (GenericResult)Engine.FinishScriptAndGetResult(jobId, i != 0)!;

            Assert.That(result.Result, Is.EqualTo("READY"));
        }

        Assert.Throws<ArgumentException>(() => Engine.FinishScriptAndGetResult(jobId));
    }

    [Test]
    public async Task Error_Will_Not_Finish_Async()
    {
        /* Termination helper. */
        var done = new TaskCompletionSource();

        ((Sink)GetService<IScriptEngineNotifySink>()).OnEvent = (method, arg) =>
        {
            /* See if script is done. */
            if (method == ScriptEngineNotifyMethods.Done)
                done.SetResult();
            else if (method == ScriptEngineNotifyMethods.Error)
                try
                {
                    Assert.That(((ScriptError)arg!).ErrorMessage, Is.EqualTo("AUTSCH"));
                }
                finally
                {
                    done.SetResult();
                }
        };

        var jobId = await Engine.StartAsync(new StartGenericScript { Name = "Throw Exception", ScriptId = AddScript("SCRIPT", Script2) }, "");

        /* Wait for the script to finish. */
        await done.Task;

        /* Check the result. */
        var result = (GenericResult)Engine.FinishScriptAndGetResult(jobId)!;
    }
}