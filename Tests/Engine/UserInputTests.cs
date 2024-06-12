using System.Text.Json;
using BlocklyNet;
using BlocklyNet.Core;
using BlocklyNet.Extensions.Builder;
using BlocklyNet.Scripting.Engine;
using BlocklyNet.Scripting.Generic;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BlocklyNetTests.Engine;

[TestFixture]
public class UserInputTests : TestEnvironment
{
    /* Just request some user input. */
    private const string Script1 = @"
        <xml xmlns=""https://developers.google.com/blockly/xml"">
        <variables>
            <variable id=""82ShC_R^Q[g,OI-C-]xP"">result</variable>
        </variables>
        <block type=""variables_set"" id=""QoWpYS4@-9wzGOL@PV+q"" x=""225"" y=""125"">
            <field name=""VAR"" id=""82ShC_R^Q[g,OI-C-]xP"">result</field>
            <value name=""VALUE"">
            <block type=""request_user_input"" id=""aO9sveYnV`}OfSx3?]W/"">
                <field name=""KEY"">Key</field>
                <field name=""TYPE"">Type</field>
                <value name=""KEY"">
                <shadow type=""text"" id=""9~xlunDmTgx|CVj$N7Qd"">
                    <field name=""TEXT"">zappy</field>
                </shadow>
                </value>
                <value name=""TYPE"">
                <shadow type=""text"" id=""_@QSx%uiAaC~VXRrk7t}"">
                    <field name=""TEXT""></field>
                </shadow>
                <block type=""text"" id=""n82I^!WwQ#l:=F/TpDZK"">
                    <field name=""TEXT"">tariffs(T0PPlus T0PMinus)</field>
                </block>
                </value>
            </block>
            </value>
        </block>
        </xml>";

    public class MyModel
    {
        public const string BlocklyName = "my_model";

        public string Data { get; set; } = null!;
    }

    /// <summary>
    /// Add custom block definitions.
    /// </summary>
    class Configurator : IParserConfiguration
    {
        /// <inheritdoc/>
        public void Configure<TParser>(BlocklyExtensions.BlockBuilder<TParser> builder) where TParser : Parser<TParser>
        {
            builder.AddModel<MyModel>(MyModel.BlocklyName, "The model");
        }
    }

    /// <inheritdoc/>
    protected override void OnSetup(IServiceCollection services)
    {
        base.OnSetup(services);

        /* Register the broadcast sink. */
        services.AddSingleton<IScriptEngineNotifySink, Sink>();

        /* Register our model in blockly. */
        services.AddSingleton<IParserConfiguration, Configurator>();
    }

    [Test]
    public async Task Can_Provide_A_User_Input_Model()
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

            /* See if user data is requested. */
            else if (method == ScriptEngineNotifyMethods.InputRequest)
            {
                var value = new MyModel { Data = "Test me!" };

                var request = (UserInputRequest)arg!;
                var engine = GetService<IScriptEngine>();

                /* Simulate serialization in real world scenario. */
                var response = new UserInputResponse
                {
                    Key = request.Key,
                    JobId = request.JobId,
                    Value = JsonSerializer.Deserialize<object>(JsonSerializer.Serialize(value, JsonUtils.JsonSettings), JsonUtils.JsonSettings),
                    ValueType = MyModel.BlocklyName
                };

                /* We may not do an immediate reply since this will run into a deadlock. */
                ThreadPool.QueueUserWorkItem((_) => engine.SetUserInput(response));
            }
        };

        var jobId = Engine.Start(new StartGenericScript { Name = "Will request user input", ScriptId = AddScript("SCRIPT", Script1) }, "");

        /* Wait for the script to finish. */
        await done.Task;

        /* Check the result. */
        var result = (GenericResult)Engine.FinishScriptAndGetResult(jobId)!;

        Assert.That(result.Result, Is.TypeOf<MyModel>());
        Assert.That(((MyModel)result.Result).Data, Is.EqualTo("Test me!"));
    }
}