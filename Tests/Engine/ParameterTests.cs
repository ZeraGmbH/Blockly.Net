using System.Text.Json;
using System.Text.Json.Nodes;
using BlocklyNet;
using BlocklyNet.Scripting.Engine;
using BlocklyNet.Scripting.Generic;
using BlocklyNet.Scripting.Logging;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BlocklyNetTests.Engine;

[TestFixture]
public class ParameterTests : TestEnvironment
{
    private const string Script1 = @"<xml xmlns=""https://developers.google.com/blockly/xml"">
        <variables>
            <variable id=""Pki@:.jOmn@n2oL_t-Gp"">result</variable>
            <variable id=""U|_7fqt4,/UY)LFejq2a"">ARG1</variable>
        </variables>
        <block type=""variables_set"" id=""k3_(:10GP)CI~R^I60tK"" x=""25"" y=""125"">
            <field name=""VAR"" id=""Pki@:.jOmn@n2oL_t-Gp"">result</field>
            <value name=""VALUE"">
            <block type=""variables_get"" id=""bgILay^BDPV}(.K2GPc0"">
                <field name=""VAR"" id=""U|_7fqt4,/UY)LFejq2a"">ARG1</field>
            </block>
            </value>
        </block>
        </xml>";

    /// <inheritdoc/>
    protected override void OnSetup(IServiceCollection services)
    {
        var builtIn = GenericScript<ScriptLoggingResult, StartGenericScript.NoopModifier>.BuiltInTypesForUnitTest;

        builtIn.Add("boolean", typeof(bool));
        builtIn.Add("email", typeof(string));
        builtIn.Add("number", typeof(double));
        builtIn.Add("string", typeof(string));

        base.OnSetup(services);

        /* Register the broadcast sink. */
        services.AddSingleton<IScriptEngineNotifySink, Sink>();
    }

    [TearDown]
    public void Teardown()
    {
        GenericScript<ScriptLoggingResult, StartGenericScript.NoopModifier>.BuiltInTypesForUnitTest.Clear();
    }

    /// <summary>
    /// Check various parameter types.
    /// </summary>
    [TestCase("number", 42)]
    [TestCase("number[]", new int[] { 42 })]
    [TestCase("string", "C")]
    [TestCase("string[]", new string[] { "A", "B" })]
    [TestCase("boolean", false)]
    [TestCase("boolean[]", new bool[] { true, false })]
    [TestCase("email", "ich@me.de")]
    [TestCase("email{}", new string[] { "ich@me.uk", "ich@me.au" })]
    public async Task Can_Use_Parameters_Async(string type, object value)
    {
        /* Termination helper. */
        var done = new TaskCompletionSource();

        /* Configure the broadcast sink. */
        ((Sink)GetService<IScriptEngineNotifySink>()).OnEvent = (method, arg) => { if (method == ScriptEngineNotifyMethods.Done) done.SetResult(); };

        var start = new StartGenericScript
        {
            Name = "Use Parameters",
            ScriptId = AddScript("PARAMS", Script1, new Parameter("ARG1", type, false)),
            Presets = { new() { Key = "ARG1", Value = value } }
        };

        var jobId = await Engine.StartAsync(
            JsonSerializer.Deserialize<StartGenericScript>(JsonSerializer.Serialize(start, JsonUtils.JsonSettings), JsonUtils.JsonSettings)!,
            ""
        );

        /* Wait for the script to finish. */
        await done.Task;

        /* Check result. */
        var result = (GenericResult)(await Engine.FinishScriptAndGetResultAsync(jobId))!;

        Assert.That(result.Result, Is.EqualTo(value));
    }
}