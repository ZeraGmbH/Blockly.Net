using BlocklyNet;
using BlocklyNet.Scripting.Debugger;
using BlocklyNet.Scripting.Engine;
using BlocklyNet.Scripting.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BlocklyNetTests.Debugger;

[TestFixture]
public class DebuggerEngineTests : TestEnvironment
{
    private class DebuggerSink() : ScriptDebugger(new NullLogger<ScriptDebugger>()), IScriptEngineNotifySink
    {
        private readonly TaskCompletionSource _done = new();

        public Task DoneTask => _done.Task;

        public Task SendAsync(ScriptEngineNotifyMethods method, object? arg1)
        {
            if (method == ScriptEngineNotifyMethods.Done || method == ScriptEngineNotifyMethods.Error)
                _done.SetResult();

            return Task.CompletedTask;
        }

        public Action<ScriptDebugContext>? OnBreak;


        public Func<ScriptDebugContext, Exception, Exception?>? OnException;


        protected override Task OnBreakAsync(ScriptDebugContext stoppedAt)
        {
            OnBreak?.Invoke(stoppedAt);

            return base.OnBreakAsync(stoppedAt);
        }

        protected override Task<Exception?> OnExceptionDetectedAsync(Exception original, ScriptDebugContext stoppedAt)
        {
            var onException = OnException;

            if (onException != null) return Task.FromResult(onException(stoppedAt, original));

            return base.OnExceptionDetectedAsync(original, stoppedAt);
        }
    }

    private DebuggerSink Debugger = null!;

    protected override void OnSetup(IServiceCollection services)
    {
        base.OnSetup(services);

        services.AddSingleton<IScriptEngineNotifySink, DebuggerSink>();
    }

    protected override void OnStartup()
    {
        Debugger = (DebuggerSink)GetService<IScriptEngineNotifySink>();

        Debugger.Enabled = true;

        Engine.Debugger = Debugger;

        base.OnStartup();
    }

    [Test]
    public async Task Can_Run_Without_Debugger_Async()
    {
        var jobId = await Engine.StartAsync(new StartGenericScript { Name = "Base for Debug Engine Tests", ScriptId = AddScript("SCRIPT", SampleScripts.DebugScript1) }, "");

        await Debugger.DoneTask;

        var result = (GenericResult)(await Engine.FinishScriptAndGetResultAsync(jobId))!;

        Assert.That(result.Result, Is.EqualTo(500500));
    }

    [TestCase(null)]
    [TestCase(10)]
    public async Task Can_Break_On_Block_And_Continue_Debugger_Async(int? stopAt)
    {
        var scriptId = AddScript("SCRIPT", SampleScripts.DebugScript1);

        Debugger.Breakpoints.Add(scriptId, "X/i3:*Zxs6B(Y!IGPoEi");

        var hits = 0;

        Debugger.OnBreak = (context) =>
        {
            if (++hits == stopAt) Debugger.Breakpoints.Remove(scriptId, "X/i3:*Zxs6B(Y!IGPoEi");
        };

        var jobId = await Engine.StartAsync(new StartGenericScript { Name = "Base for Debug Engine Tests", ScriptId = scriptId }, "");

        await Debugger.DoneTask;

        var result = (GenericResult)(await Engine.FinishScriptAndGetResultAsync(jobId))!;

        Assert.That(result.Result, Is.EqualTo(500500));
        Assert.That(hits, Is.EqualTo(stopAt ?? 1000));
    }

    [TestCase(null)]
    [TestCase(3000)]
    public async Task Can_Single_Step_Debugger_Async(int? stopAt)
    {
        Debugger.Continue(ScriptDebugContinueModes.StepInto);

        var hits = 0;

        Debugger.OnBreak = (stoppedAt) =>
        {
            if (++hits != stopAt) Debugger.Continue(ScriptDebugContinueModes.StepInto, stoppedAt);
        };

        var scriptId = AddScript("SCRIPT", SampleScripts.DebugScript1);

        var jobId = await Engine.StartAsync(new StartGenericScript { Name = "Base for Debug Engine Tests", ScriptId = scriptId }, "");

        await Debugger.DoneTask;

        var result = (GenericResult)(await Engine.FinishScriptAndGetResultAsync(jobId))!;

        Assert.That(result.Result, Is.EqualTo(500500));
        Assert.That(hits, Is.EqualTo(stopAt ?? 4006));
    }

    [Test]
    public async Task Can_Stop_On_Start_Async()
    {
        Debugger.Continue(ScriptDebugContinueModes.StepInto);

        var hitFirst = false;

        Debugger.OnBreak = (context) =>
        {
            Assert.That(context.Block.Id, Is.EqualTo("~-@g:c_wcw/=l7I4Z$X9"));

            hitFirst = true;
        };

        var scriptId = AddScript("SCRIPT", SampleScripts.DebugScript1);

        var jobId = await Engine.StartAsync(new StartGenericScript { Name = "Base for Debug Engine Tests", ScriptId = scriptId }, "");

        await Debugger.DoneTask;

        var result = (GenericResult)(await Engine.FinishScriptAndGetResultAsync(jobId))!;

        Assert.That(result.Result, Is.EqualTo(500500));
        Assert.That(hitFirst, Is.True);
    }

    [TestCase(300)]
    public async Task Can_Inspect_Variables_Debugger_Async(int testAt)
    {
        var scriptId = AddScript("SCRIPT", SampleScripts.DebugScript1);

        Debugger.Breakpoints.Add(scriptId, "X/i3:*Zxs6B(Y!IGPoEi");

        var hits = 0;

        Debugger.OnBreak = (context) =>
        {
            if (++hits != testAt) return;

            Debugger.Breakpoints.Remove(scriptId, "X/i3:*Zxs6B(Y!IGPoEi");

            var variables = context.GetVariables();

            Assert.That(variables, Has.Count.EqualTo(1));

            var scope = variables[0];

            Assert.That(scope.ScriptId, Is.EqualTo(scriptId));
            Assert.That(scope.Variables, Has.Count.EqualTo(2));

            var iVar = scope.Variables.Single(v => v.Name == "i");
            var rVar = scope.Variables.Single(v => v.Name == "result");

            Assert.That(iVar.Type, Is.Null);
            Assert.That(iVar.Value, Is.EqualTo("300"));
            Assert.That(rVar.Type, Is.Null);
            Assert.That(rVar.Value, Is.EqualTo("44850"));

            scope.SetVariable("result", "0");
        };

        var jobId = await Engine.StartAsync(new StartGenericScript { Name = "Base for Debug Engine Tests", ScriptId = scriptId }, "");

        await Debugger.DoneTask;

        var result = (GenericResult)(await Engine.FinishScriptAndGetResultAsync(jobId))!;

        Assert.That(result.Result, Is.EqualTo(500500 - 44850));
        Assert.That(hits, Is.EqualTo(testAt));
    }

    [TestCase(ScriptDebugContinueModes.StepInto, "lIvo!.Jrw(mvKuaEVo2=")]
    [TestCase(ScriptDebugContinueModes.StepOver, "Kb__-_**cI=OxUIZg9yW")]
    public async Task Can_Stop_On_Volative_Async(ScriptDebugContinueModes mode, string nextBlockId)
    {
        var scriptId = AddScript("SCRIPT", SampleScripts.DebugScript1);

        Debugger.Breakpoints.Add(scriptId, "~-@g:c_wcw/=l7I4Z$X9");

        string? next = null;

        Debugger.OnBreak = (stoppedAt) =>
        {
            if (next != null)
                Assert.That(stoppedAt.Block.Id, Is.EqualTo(nextBlockId));
            else if (stoppedAt.Block.Id == "~-@g:c_wcw/=l7I4Z$X9")
            {
                Debugger.Continue(mode, stoppedAt);

                next = nextBlockId;
            }
        };

        var jobId = await Engine.StartAsync(new StartGenericScript { Name = "Base for Debug Engine Tests", ScriptId = scriptId }, "");

        await Debugger.DoneTask;

        var result = (GenericResult)(await Engine.FinishScriptAndGetResultAsync(jobId))!;

        Assert.That(result.Result, Is.EqualTo(500500));
    }

    [TestCase(false, 2)]
    [TestCase(true, 1)]
    public async Task Can_Intercept_Exception_Async(bool ignore, int expected)
    {
        Debugger.Breakpoints.BreakOnExceptions = true;

        Debugger.OnException = (context, original) =>
        {
            Assert.That(original.Message, Is.EqualTo("Force to 2"));

            return ignore ? null : original;
        };

        var scriptId = AddScript("SCRIPT", SampleScripts.DebugScript2);

        var jobId = await Engine.StartAsync(new StartGenericScript { Name = "Base for Debug Engine Tests", ScriptId = scriptId }, "");

        await Debugger.DoneTask;

        var result = (GenericResult)(await Engine.FinishScriptAndGetResultAsync(jobId))!;

        Assert.That(result.Result, Is.EqualTo(expected));
    }

    [Test]
    public async Task Can_Step_Out_Of_Procedure_Async()
    {
        var scriptId = AddScript("SCRIPT", SampleScripts.DebugScript3);

        Debugger.Breakpoints.Add(scriptId, "G5]l)A/yzjQbrq)Cr57t");

        Debugger.OnBreak = stoppedAt =>
        {
            if (stoppedAt.Position.BlockId == "G5]l)A/yzjQbrq)Cr57t")
            {
                var variables = stoppedAt.GetVariables();

                Assert.That(variables, Has.Count.EqualTo(3));

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(variables[0].ScriptId, Is.EqualTo(scriptId));
                    Assert.That(variables[0].Procedure, Is.EqualTo("generate"));
                    Assert.That(variables[1].ScriptId, Is.EqualTo(scriptId));
                    Assert.That(variables[1].Procedure, Is.EqualTo("sumTo"));
                    Assert.That(variables[2].ScriptId, Is.EqualTo(scriptId));
                    Assert.That(variables[2].Procedure, Is.Null);
                }

                Debugger.Breakpoints[scriptId, "G5]l)A/yzjQbrq)Cr57t"]!.Enabled = false;

                Debugger.Continue(ScriptDebugContinueModes.StepOut, stoppedAt);
            }
            else if (stoppedAt.Position.BlockId == ";g!0eq5)ITw{V(s*tSW/")
                Debugger.Continue(ScriptDebugContinueModes.StepOut, stoppedAt);
            else
                Assert.That(stoppedAt.Position.BlockId, Is.EqualTo("sNfTZ[N@*YOpXCOZUZEc"));
        };

        var jobId = await Engine.StartAsync(new StartGenericScript { Name = "Base for Debug Engine Tests", ScriptId = scriptId }, "");

        await Debugger.DoneTask;

        var result = (GenericResult)(await Engine.FinishScriptAndGetResultAsync(jobId))!;

        Assert.That(result.Result, Is.EqualTo(1610));
    }

    [Test]
    public async Task Can_Step_Out_Of_Nested_Script_Async()
    {
        var innerId = AddScript("SCRIPTI", SampleScripts.DebugScript4Inner);
        var outerId = AddScript("SCRIPTO", SampleScripts.DebugScript4Outer(innerId));

        Debugger.Breakpoints.Add(innerId, "Z*dM$.4KJX*@?2L#Ll~^");

        Debugger.OnBreak = stoppedAt =>
        {
            if (stoppedAt.BlockId == "Z*dM$.4KJX*@?2L#Ll~^")
            {
                Debugger.Breakpoints[innerId, "Z*dM$.4KJX*@?2L#Ll~^"]!.Enabled = false;

                Debugger.Continue(ScriptDebugContinueModes.LeaveScript, stoppedAt);
            }
            else
                Assert.That(stoppedAt.BlockId, Is.EqualTo("+%/-8RI^@6ZZo+~J#rV*"));
        };

        var jobId = await Engine.StartAsync(new StartGenericScript { Name = "Base for Debug Engine Tests", ScriptId = outerId }, "");

        await Debugger.DoneTask;

        var result = (GenericResult)(await Engine.FinishScriptAndGetResultAsync(jobId))!;

        Assert.That(result.Result, Is.EqualTo(-55));
    }

    [Test]
    public async Task Can_Run_Parallel_Scripts_Async()
    {
        var innerId = AddScript("SCRIPTI", SampleScripts.DebugScript5Inner, new Parameter("Hint", "", true));
        var outerId = AddScript("SCRIPTO", SampleScripts.DebugScript5Outer(innerId));

        Debugger.Breakpoints.Add(innerId, "/:N_iDv3|JbFCX`Ac/+?");

        HashSet<string> wait = ["\"A\"", "\"B\"", "\"C\""];

        var hits = 0;

        Debugger.OnBreak = (stoppedAt) =>
        {
            if (wait.Count > 0)
            {
                hits++;

                Assert.That(stoppedAt.ScriptId, Is.EqualTo(innerId));
                Assert.That(stoppedAt.BlockId, Is.EqualTo("/:N_iDv3|JbFCX`Ac/+?"));

                var var = stoppedAt.GetVariables()[0].Variables.Single(v => v.Name == "Hint");

                wait.Remove(var.Value ?? string.Empty);

                if (wait.Count < 1)
                {
                    Debugger.Breakpoints.Remove(innerId, "/:N_iDv3|JbFCX`Ac/+?");
                    Debugger.Continue(ScriptDebugContinueModes.LeaveScript, stoppedAt);
                }
            }
            else
            {
                Assert.That(stoppedAt.ScriptId, Is.EqualTo(outerId));
                Assert.That(stoppedAt.BlockId, Is.EqualTo("oN]Q)P-|fWu2?kZJ%IE0"));

                var var = stoppedAt.GetVariables()[0].Variables.Single(v => v.Name == "parallel");
                var value = JsonSerializer.Deserialize<JsonArray>(var.Value ?? "[]", JsonUtils.JsonSettings);

                Assert.That(value, Has.Count.EqualTo(3));
            }
        };

        var jobId = await Engine.StartAsync(new StartGenericScript { Name = "Base for Debug Engine Tests", ScriptId = outerId }, "");

        await Debugger.DoneTask;

        var result = (GenericResult)(await Engine.FinishScriptAndGetResultAsync(jobId))!;

        Assert.That(hits, Is.GreaterThanOrEqualTo(3).And.LessThanOrEqualTo(30));
        Assert.That(result.Result, Is.EqualTo("A ** A ** A ** A ** A ** A ** A ** A ** A ** A ++ B ** B ** B ** B ** B ** B ** B ** B ** B ** B ++ C ** C ** C ** C ** C ** C ** C ** C ** C ** C"));
    }

    [Test]
    public async Task Can_Run_To_End_Of_Script_Async()
    {
        Debugger.Breakpoints.BreakOnEndOfScript = true;

        Debugger.OnBreak = (context) =>
        {
            var var = context.GetVariables()[0].Variables.Single(v => v.Name == "result");

            Assert.That(var.Value, Is.EqualTo("500500"));
        };

        var jobId = await Engine.StartAsync(new StartGenericScript { Name = "Base for Debug Engine Tests", ScriptId = AddScript("SCRIPT", SampleScripts.DebugScript1) }, "");

        await Debugger.DoneTask;

        var result = (GenericResult)(await Engine.FinishScriptAndGetResultAsync(jobId))!;

        Assert.That(result.Result, Is.EqualTo(500500));
    }

    [TestCase("$%v6=0*|vNRKgbn(|(n;")]
    [TestCase("{U1[ES?Fxa;3EP2%Sk-F")]
    public async Task Can_Step_Over_Async(string at)
    {
        var scriptId = AddScript("SCRIPT", SampleScripts.DebugScript3);

        Debugger.Breakpoints.Add(scriptId, at);

        Debugger.OnBreak = (stoppedAt) =>
        {
            if (stoppedAt.Position.BlockId == at)
            {
                Debugger.Breakpoints.Remove(scriptId, at);

                Debugger.Continue(ScriptDebugContinueModes.StepOver, stoppedAt);
            }
            else
                Assert.That(stoppedAt.Position.BlockId, Is.EqualTo(";g!0eq5)ITw{V(s*tSW/"));
        };

        var jobId = await Engine.StartAsync(new StartGenericScript { Name = "Base for Debug Engine Tests", ScriptId = scriptId }, "");

        await Debugger.DoneTask;

        var result = (GenericResult)(await Engine.FinishScriptAndGetResultAsync(jobId))!;

        Assert.That(result.Result, Is.EqualTo(1610));
    }
}