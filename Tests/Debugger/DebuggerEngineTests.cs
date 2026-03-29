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
        public readonly List<ScriptDebugContext> Breaks = [];

        private readonly TaskCompletionSource _done = new();

        public Task DoneTask => _done.Task;

        public Task SendAsync(ScriptEngineNotifyMethods method, object? arg1)
        {
            if (method == ScriptEngineNotifyMethods.Done || method == ScriptEngineNotifyMethods.Error)
                _done.SetResult();

            return Task.CompletedTask;
        }

        public Action<ScriptDebugContext, IScriptBreakpoint>? OnHit;

        public Action<ScriptDebugContext>? OnStep;

        public Action<ScriptDebugContext>? OnStart;

        public Action<ScriptDebugContext>? OnVolatile;

        public Func<ScriptDebugContext, Exception, Exception?>? OnException;

        protected override Task OnBreakpointHitAsync(IScriptBreakpoint bp)
        {
            Breaks.Add(Context);

            OnHit?.Invoke(Context, bp);

            return base.OnBreakpointHitAsync(bp);
        }

        protected override Task OnSingleStepAsync()
        {
            Breaks.Add(Context);

            OnStep?.Invoke(Context);

            return base.OnSingleStepAsync();
        }

        protected override Task OnFirstBlockAsync()
        {
            Breaks.Add(Context);

            OnStart?.Invoke(Context);

            return base.OnFirstBlockAsync();
        }

        protected override Task OnVolatileStopAsync()
        {
            Breaks.Add(Context);

            OnVolatile?.Invoke(Context);

            return base.OnVolatileStopAsync();
        }

        protected override Task<Exception?> OnExceptionDetectedAsync(Exception original)
        {
            if (OnException != null) return Task.FromResult(OnException(Context, original));

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

        ((IScriptSite)Engine).SetDebugger(Debugger);

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

        Debugger.OnHit = (context, bp) =>
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
        Debugger.SingleStep = true;

        var hits = 0;

        Debugger.OnStep = (context) =>
        {
            if (++hits == stopAt) Debugger.SingleStep = false;
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
        Debugger.StopOnStart = true;

        var hitFirst = false;

        Debugger.OnStart = (context) =>
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

        Debugger.OnHit = (context, bp) =>
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
        };

        var jobId = await Engine.StartAsync(new StartGenericScript { Name = "Base for Debug Engine Tests", ScriptId = scriptId }, "");

        await Debugger.DoneTask;

        var result = (GenericResult)(await Engine.FinishScriptAndGetResultAsync(jobId))!;

        Assert.That(result.Result, Is.EqualTo(500500));
        Assert.That(hits, Is.EqualTo(testAt));
    }

    [Test]
    public async Task Can_Stop_On_Volative_Async()
    {
        var scriptId = AddScript("SCRIPT", SampleScripts.DebugScript1);

        Debugger.Breakpoints.Add(scriptId, "~-@g:c_wcw/=l7I4Z$X9");

        Debugger.OnHit = (context, bp) =>
        {
            Assert.That(context.Block.Id, Is.EqualTo("~-@g:c_wcw/=l7I4Z$X9"));

            Debugger.Breakpoints.RunTo(scriptId, context.Block.Next!.Id);
        };

        Debugger.OnVolatile = (context) =>
        {
            Assert.That(context.Block.Id, Is.EqualTo("Kb__-_**cI=OxUIZg9yW"));
        };

        var jobId = await Engine.StartAsync(new StartGenericScript { Name = "Base for Debug Engine Tests", ScriptId = scriptId }, "");

        await Debugger.DoneTask;

        var result = (GenericResult)(await Engine.FinishScriptAndGetResultAsync(jobId))!;

        Assert.That(result.Result, Is.EqualTo(500500));
        Assert.That(Debugger.Breaks, Has.Count.EqualTo(2));
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