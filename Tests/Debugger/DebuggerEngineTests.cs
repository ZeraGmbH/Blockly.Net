using BlocklyNet.Scripting.Debugger;
using BlocklyNet.Scripting.Engine;
using BlocklyNet.Scripting.Generic;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BlocklyNetTests.Debugger;

[TestFixture]
public class DebuggerEngineTests : TestEnvironment
{
    private class DebuggerSink : ScriptDebugger, IScriptEngineNotifySink
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


        protected override Task OnBreakAsync()
        {
            OnBreak?.Invoke(StoppedAt!);

            return base.OnBreakAsync();
        }

        protected override Task<Exception?> OnExceptionDetectedAsync(Exception original)
        {
            var onException = OnException;

            if (onException != null) return Task.FromResult(onException(StoppedAt!, original));

            return base.OnExceptionDetectedAsync(original);
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

        Engine.SetDebugger(Debugger);

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

        Debugger.OnBreak = (context) =>
        {
            if (++hits != stopAt) Debugger.Continue(ScriptDebugContinueModes.StepInto);
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

            var variables = Debugger.GetVariables();

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

        Debugger.OnBreak = (context) =>
        {
            if (next != null)
                Assert.That(context.Block.Id, Is.EqualTo(nextBlockId));
            else if (context.Block.Id == "~-@g:c_wcw/=l7I4Z$X9")
            {
                Debugger.Continue(mode);

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
}