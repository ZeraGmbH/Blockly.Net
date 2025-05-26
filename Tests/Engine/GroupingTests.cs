using BlocklyNet.Core;
using BlocklyNet.Extensions.Builder;
using BlocklyNet.Scripting.Engine;
using BlocklyNet.Scripting.Generic;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BlocklyNetTests.Engine;

[TestFixture]
public class GroupingTests : TestEnvironment
{
    private class ResultClass
    {
        public string Name { get; set; } = null!;
    }

    class Configurator : IParserConfiguration
    {
        /// <inheritdoc/>
        public void Configure<TParser>(BlocklyExtensions.BlockBuilder<TParser> builder) where TParser : Parser<TParser>
        {
            builder.AddModel<ResultClass>("result_class", "The result class");
        }
    }

    /// <inheritdoc/>
    protected override void OnSetup(IServiceCollection services)
    {
        base.OnSetup(services);

        services.AddSingleton<IScriptEngineNotifySink, Sink>();
        services.AddSingleton<IParserConfiguration, Configurator>();
    }

    /* Just request some user input. */
    private const string Script1 = @"
        <xml xmlns=""https://developers.google.com/blockly/xml"">
        <variables>
            <variable type=""group_execution_status"">StatusVar</variable>
            <variable type=""group_execution_result_type"">ResultVar</variable>
        </variables>            
        <block type=""execute_group"" id=""Outer"">
            <field name=""NAME"">Outer</field>
            <field name=""STATUSVAR"">StatusVar</field>
            <field name=""RESULTVAR"">ResultVar</field>
            <value name=""RESULT"">                
                <block type=""group_execution_result"" id=""Outer.Result"">
                    <value name=""Result"">                
                        <block type=""group_execution_result_type"" id=""Outer.Result.Value"">
                            <field name=""VALUE"">Failed</field>
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
                    <field name=""NAME"">Inner</field>
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

    public const string Script2 = @"<xml xmlns=""https://developers.google.com/blockly/xml"">
        <variables>
            <variable id=""jRl,$B07:ak%A+}Ae`7e"">groupStatus</variable>
            <variable id=""jtUShfEj2=2oQNw;n(!5"">multi</variable>
            <variable type=""result_class"" id=""mbWJ:X%U!e`RnjZ~O6Y+"">single</variable>
            <variable id=""VJ*txd)2=H1N[7|:!70f"">result</variable>
        </variables>
        <block type=""execute_group"" id=""p%pFJy;4En4FWUcOI{3."" x=""175"" y=""125"">
            <field name=""NAME"">Test</field>
            <field name=""DETAILS"">Details</field>
            <field name=""RESULT"">Result</field>
            <field name=""STATUSVAR"" id=""jRl,$B07:ak%A+}Ae`7e"">groupStatus</field>
            <field name=""RESULTVAR"" id=""jtUShfEj2=2oQNw;n(!5"">multi</field>
            <field name=""RESULTVARELEMENT"" id=""mbWJ:X%U!e`RnjZ~O6Y+"">single</field>
            <statement name=""DO"">
                <block type=""variables_set"" id=""TDg8],OZU#M5OYl{5P)w"">
                    <field name=""VAR"" id=""mbWJ:X%U!e`RnjZ~O6Y+"" variabletype=""result_class"">single</field>
                    <value name=""VALUE"">
                        <block type=""result_class"" id=""4`]cxJxHEq[XZ7IzfV=K"">
                            <field name=""Name"">Name</field>
                            <value name=""Name"">
                                <shadow type=""text"" id=""#AUq9`s!U3g~#=g}_?q{"">
                                    <field name=""TEXT"">Pete</field>
                                </shadow>
                            </value>
                        </block>
                    </value>
                </block>
            </statement>
            <value name=""RESULT"">
                <block type=""group_execution_result"" id=""{bNS9bT_NKk/+`.3^;u,"">
                    <field name=""Type"">Type</field>
                    <field name=""Result"">Result</field>
                    <value name=""Type"">
                        <shadow type=""group_execution_result_type"" id=""0z!Q^}TyTsLl[egdewE-"">
                            <field name=""VALUE"">Succeeded</field>
                        </shadow>
                    </value>
                    <value name=""Result"">
                        <block type=""lists_create_with"" id=""/5AJxpVmWw$!@T=$SOMU"">
                            <mutation items=""1""></mutation>
                            <value name=""ADD0"">
                                <block type=""variables_get"" id=""?H]E2W8j55ZApi2mF%mn"">
                                    <field name=""VAR"" id=""mbWJ:X%U!e`RnjZ~O6Y+"" variabletype=""result_class"">single</field>
                                </block>
                            </value>
                        </block>
                    </value>
                </block>
            </value>
            <next>
                <block type=""variables_set"" id=""M]P2{$xD#$_JL`KtOX!1"">
                    <field name=""VAR"" id=""VJ*txd)2=H1N[7|:!70f"">result</field>
                    <value name=""VALUE"">
                        <block type=""variables_get"" id=""a8[Ds1i,S+bvMdA_F9b}"">
                            <field name=""VAR"" id=""jtUShfEj2=2oQNw;n(!5"">multi</field>
                        </block>
                    </value>
                </block>
            </next>
        </block>
        </xml>";

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
        var result = (GenericResult)(await Engine.FinishScriptAndGetResultAsync(jobId))!;

        Assert.That((IList<object?>)result.Result, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task Can_Run_Groups_With_Array_Result_Async()
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

        var jobId = await Engine.StartAsync(new StartGenericScript { Name = "Run Groups", ScriptId = AddScript("SCRIPT", Script2) }, "");

        /* Wait for the script to finish. */
        await done.Task;

        /* Check the result. */
        var result = (GenericResult)(await Engine.FinishScriptAndGetResultAsync(jobId))!;
        var list = (IList<object?>)result.Result;

        Assert.That(list, Has.Count.EqualTo(1));

        Assert.Multiple(() =>
        {
            Assert.That(list[0], Is.TypeOf<ResultClass>());
            Assert.That(((ResultClass)list[0]!).Name, Is.EqualTo("Pete"));
        });
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
        Assert.That(hash.Single(), Is.EqualTo("5E-C3-38-B4-A9-E4-85-C1-16-CD-73-44-01-E1-36-93-2A-7C-65-95-92-4A-23-43-43-4F-DE-E8-70-CE-8B-C7"));

        /* Validate status. */
        Assert.That(status, Is.Not.Null);
        Assert.That(status.GroupStatus, Has.Count.EqualTo(2));

        Assert.Multiple(() =>
        {
            Assert.That(status.GroupStatus[0].Key, Is.EqualTo("Outer"));
            Assert.That(status.GroupStatus[1].Key, Is.EqualTo("Inner"));
        });

        /* Check the result. */
        var result = (GenericResult)(await Engine.FinishScriptAndGetResultAsync(jobId))!;

        Assert.That((IList<object?>)result.Result, Has.Count.EqualTo(2));
    }
}