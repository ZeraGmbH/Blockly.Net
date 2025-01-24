using BlocklyNet.Scripting.Engine;
using BlocklyNet.Scripting.Generic;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BlocklyNetTests.Engine;

[TestFixture]
public class RestartTests : TestEnvironment
{
    private const string Script1 = @"
        <xml xmlns=""https://developers.google.com/blockly/xml"">
        <variables>
            <variable>result</variable>
        </variables>
        <block type=""variables_set"">
            <field name=""VAR"">result</field>
            <value name=""VALUE"">
                <block type=""get_date_time"">
                    <value name=""FORMAT"">
                        <block type=""text"">
                            <field name=""TEXT"">yyyy-MM-dd HH:mm:ss.fff</field>
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

    [Test]
    public async Task Can_Restart_Script_Async()
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

        var jobId1 = await Engine.StartAsync(new StartGenericScript { Name = "Restart", ScriptId = AddScript("SCRIPT", Script1) }, "");

        /* Wait for the script to finish. */
        await done.Task;

        var result1 = (GenericResult)(await Engine.FinishScriptAndGetResultAsync(jobId1, true))!;

        await Task.Delay(100);

        /* Run again. */
        done = new TaskCompletionSource();

        var jobId2 = await Engine.RestartAsync([]);

        /* Wait for the script to finish. */
        await done.Task;

        var result2 = (GenericResult)(await Engine.FinishScriptAndGetResultAsync(jobId2))!;

        Assert.That((string)result2.Result, Is.GreaterThan((string)result1.Result));
    }
}