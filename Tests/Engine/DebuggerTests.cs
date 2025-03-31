using BlocklyNet.Core.Model;
using BlocklyNet.Scripting.Debugger;
using BlocklyNet.Scripting.Engine;
using BlocklyNet.Scripting.Generic;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BlocklyNetTests.Engine;

[TestFixture]
public class DebuggerTests : TestEnvironment
{
    class Debugger : IScriptDebugger
    {
        public readonly List<string> Actions = [];

        public Task InterceptAsync(Block block, Context context, ScriptDebuggerStopReason reason)
        {
            Actions.Add($"{reason} {block.Id}");

            foreach (var variable in context.Variables)
                Actions.Add($"\t{variable.Key}={variable.Value?.ToString()}");

            return Task.CompletedTask;
        }

        public void ScriptFinished(Exception? e)
        {
            Actions.Add($"DONE {e?.Message}");
        }
    }

    private const string Script1 = @"
        <xml xmlns=""https://developers.google.com/blockly/xml"">
        <variables>
            <variable>result</variable>
        </variables>
        <block type=""variables_set"" id=""SET"">
            <field name=""VAR"">result</field>
            <value name=""VALUE"">
                <block type=""get_date_time"" id=""NOW"">
                    <value name=""FORMAT"">
                        <block type=""text"" id=""FMT"">
                            <field name=""TEXT"">XTEST</field>
                        </block>
                    </value>
                </block>
            </value>
            <next>
                <block type=""throw_exception"" id=""THROW"">
                    <value name=""MESSAGE"">
                        <block type=""text"" id=""ERROR"">
                            <field name=""TEXT"">Break it</field>
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
    public async Task Can_Debug_Script_Async()
    {
        var debuggerSite = (IScriptSite)Engine;
        var debugger = new Debugger();

        debuggerSite.SetDebugger(debugger);

        try
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

            var jobId = await Engine.StartAsync(new StartGenericScript { Name = "Debug", ScriptId = AddScript("SCRIPT", Script1) }, "");

            /* Wait for the script to finish. */
            await done.Task;

            await Engine.FinishScriptAndGetResultAsync(jobId, true);
        }
        finally
        {
            debuggerSite.SetDebugger(null);
        }

        Assert.That(debugger.Actions, Is.EqualTo([
            /* Variable initializer. */
            "Enter ",
            "Leave ",
            "\tresult=",
            "Finish ",
            "\tresult=",
            /* Our code. */
            "Enter SET",
            "\tresult=",
            "Enter NOW",
            "\tresult=",
            "Enter FMT",
            "\tresult=",
            "Finish FMT",
            "\tresult=",
            "Finish NOW",
            "\tresult=",
            "Leave SET",
            "\tresult=XTEST",
            "Enter THROW",
            "\tresult=XTEST",
            "Enter ERROR",
            "\tresult=XTEST",
            "Finish ERROR",
            "\tresult=XTEST",
            "DONE Break it"
        ]));
    }
}