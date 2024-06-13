using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using BlocklyNet;
using BlocklyNet.Core;
using BlocklyNet.Extensions.Builder;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BlocklyNetTests.Customization;

/// <summary>
/// Sample enumeration.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SampleEnum
{
    One = 1,
    Two = 2,
    Three = 3,
}

/// <summary>
/// Some leaf model.
/// </summary>
public class InnerClass
{
    public string Name { get; set; } = null!;

    public SampleEnum What { get; set; }
}

/// <summary>
/// Some higher level mode accessing the leaf model.
/// </summary>
public class OuterClass
{
    public string Name { get; set; } = null!;

    public SampleEnum What { get; set; }

    public List<InnerClass> Inner { get; set; } = [];
}

/// <summary>
/// Array of dictionaries.
/// </summary>
public class ListDictClass
{
    public List<Dictionary<SampleEnum, bool>> TheList { get; set; } = [];
}

[TestFixture]
public class ModelAndEnumTests : TestEnvironment
{
    private static readonly Regex ClearWhiteSpaces = new(@"(\n|\s)");

    /// <summary>
    /// Add custom block definitions.
    /// </summary>
    class Configurator : IParserConfiguration
    {
        /// <inheritdoc/>
        public void Configure<TParser>(BlocklyExtensions.BlockBuilder<TParser> builder) where TParser : Parser<TParser>
        {
            /* Enums first. */
            builder.AddEnum<SampleEnum>("sample_enum", "My example enumeration");

            /* Model classes leaves first. */
            builder.AddModel<InnerClass>("inner_class", "The inner model");
            builder.AddModel<OuterClass>("outer_class", "The outer model");
            builder.AddModel<ListDictClass>("list_and_dictionary", "Complex list with dictionary");
        }
    }

    /// <inheritdoc/>
    protected override void OnSetup(IServiceCollection services)
    {
        base.OnSetup(services);

        services.AddSingleton<IParserConfiguration, Configurator>();
    }

    /// <summary>
    /// See if the runtime can provide the custom enumeration and the
    /// mapping between display name and block type.
    /// </summary>
    [Test]
    public void Can_Read_Enumerations()
    {
        var provider = GetService<IConfigurationService>();
        var enums = provider.EnumerationNames.ToDictionary(e => e.Name);

        Assert.That(enums["My example enumeration"].Type, Is.EqualTo("sample_enum"));
    }

    /// <summary>
    /// See if the runtime can provide the custom models and the
    /// mapping between display name and block type.
    /// </summary>
    [Test]
    public void Can_Read_Models()
    {
        var provider = GetService<IConfigurationService>();
        var models = provider.ModelNames.ToDictionary(e => e.Name);

        Assert.That(models["The inner model"].Type, Is.EqualTo("inner_class"));
        Assert.That(models["The outer model"].Type, Is.EqualTo("outer_class"));
    }

    /// <summary>
    /// See if the automatically generated block definitions can be read.
    /// </summary>
    [Test]
    public void Can_Read_Block_Definitions()
    {
        var provider = GetService<IConfigurationService>();
        var defs1 = provider.Configuration;
        var defs2 = provider.Configuration;

        /* For performance reasons the configuration should be cached. */
        Assert.That(defs1, Is.SameAs(defs2));
    }

    /// <summary>
    /// See if the automatically generated block definitions for our
    /// enumeration can be read.
    /// </summary>
    [Test]
    public void Can_Read_Enum_Definition()
    {
        var provider = GetService<IConfigurationService>();

        /* The enumeration block. */
        var myEnum = provider.Configuration["models"]!.AsArray().First(j => j!["type"]?.GetValue<string>() == "sample_enum");

        Assert.That(myEnum, Is.Not.Null);

        var field = myEnum["args0"]![0]!;

        Assert.That(field["type"]!.GetValue<string>(), Is.EqualTo("field_dropdown"));
        Assert.That(field["name"]!.GetValue<string>(), Is.EqualTo("VALUE"));

        var options = (JsonArray)field["options"]!;
        var values = options.Select(j => j![0]!.GetValue<string>()).ToArray();

        Assert.That(values, Is.EquivalentTo(new[] { "One", "Three", "Two" }));

        /* The enumeration toolbox entry. */
        var toolbox = (JsonArray)provider.Configuration["toolbox"]![0]!["contents"]!;
        var enums = toolbox.Single(j => j!["name"]?.GetValue<string>() == "$Enum$")!;
        var myToolbox = ((JsonArray)enums["contents"]!).Single(j => j!["type"]!.GetValue<string>() == "sample_enum")!;

        Assert.That(myToolbox["_name"]!.GetValue<string>(), Is.EqualTo("My example enumeration"));
    }

    /// <summary>
    /// See if the automatically generated block definitions for one of
    /// our models can be read.
    /// </summary>
    [Test]
    public void Can_Read_Model_Definition()
    {
        var provider = GetService<IConfigurationService>();

        /* The model block. */
        var myModel = provider.Configuration["models"]!.AsArray().First(j => j!["type"]?.GetValue<string>() == "outer_class");

        Assert.That(myModel, Is.Not.Null);

        /* See if the required typing is set correctly. */
        var args = (JsonArray)myModel["args0"]!;
        var inner = args.Single(j => j!["type"]!.GetValue<string>() == "input_value" && j!["name"]!.GetValue<string>() == "Inner")!;
        var check = (JsonArray)inner["check"]!;
        var types = check.Select(j => j!.GetValue<string>()).ToArray();

        Assert.That(types, Is.EquivalentTo(new[] { "Array", "Array(inner_class)" }));

        /* The model toolbox entry. */
        var toolbox = (JsonArray)provider.Configuration["toolbox"]![0]!["contents"]!;
        var models = toolbox.Single(j => j!["name"]?.GetValue<string>() == "$Models$")!;
        var myToolbox = ((JsonArray)models["contents"]!).Single(j => j!["type"]!.GetValue<string>() == "outer_class")!;

        Assert.That(myToolbox["_name"]!.GetValue<string>(), Is.EqualTo("The outer model"));
    }

    /// <summary>
    /// See if the automatically generated block definitions for 
    /// a library provided block can be read.
    /// </summary>
    [Test]
    public void Can_Read_Delay_Definition()
    {
        var provider = GetService<IConfigurationService>();

        /* The model block. */
        var theModel = provider.Configuration["blocks"]!.AsArray().First(j => j!["type"]?.GetValue<string>() == "delay");

        Assert.That(theModel, Is.Not.Null);

        /* See if the required typing is set correctly. */
        var args = (JsonArray)theModel["args0"]!;
        var inner = args.Single(j => j!["type"]!.GetValue<string>() == "input_value" && j!["name"]!.GetValue<string>() == "DELAY")!;

        Assert.That(inner["check"]!.GetValue<string>(), Is.EqualTo("Number"));

        /* The model toolbox entry. */
        var toolbox = (JsonArray)provider.Configuration["toolbox"]![0]!["contents"]!;
        var theToolbox = toolbox.Single(j => j!["type"]?.GetValue<string>() == "delay")!;

        Assert.That(theToolbox["_name"]!.GetValue<string>(), Is.EqualTo("Delay"));
    }

    /// <summary>
    /// Check the construction of a dictionary nested inside a list.
    /// </summary>
    [Test]
    public void Can_Create_A_Dictionary_Listed_In_An_Array()
    {
        var provider = GetService<IConfigurationService>();

        /* The model block. */
        var myModel = provider.Configuration["models"]!.AsArray().First(j => j!["type"]?.GetValue<string>() == "list_and_dictionary");
        var modelDef = JsonSerializer.Serialize(myModel, JsonUtils.JsonSettings);

        var modelBlock = @"{ 
            ""args0"":[
                { ""type"":""input_dummy"" },
                { ""type"":""field_label_serializable"", ""name"":""TheList"",""text"":""TheList""},
                { ""type"":""input_value"",""name"":""TheList"",""check"":[""Array(list_and_dictionary_TheList)"", ""Array""]}
            ],
            ""message0"":""Complex list with dictionary %1 %2 %3"",
            ""output"":""list_and_dictionary"",
            ""tooltip"":""Complex list with dictionary"",
            ""type"":""list_and_dictionary""
        }";

        Assert.That(ClearWhiteSpaces.Replace(modelDef, ""), Is.EqualTo(ClearWhiteSpaces.Replace(modelBlock, "")));

        /* The dictionary block. */
        var theDict = provider.Configuration["models"]!.AsArray().First(j => j!["type"]?.GetValue<string>() == "list_and_dictionary_TheList");
        var dictDef = JsonSerializer.Serialize(theDict, JsonUtils.JsonSettings);

        var dictBlock = @"{
            ""args0"":[
                {""type"":""input_dummy""},
                {""type"":""field_label_serializable"",""name"":""One"",""text"":""One""},
                {""type"":""input_value"",""name"":""One"",""check"":""Boolean""},
                {""type"":""field_label_serializable"",""name"":""Two"",""text"":""Two""},
                {""type"":""input_value"",""name"":""Two"",""check"":""Boolean""},
                {""type"":""field_label_serializable"",""name"":""Three"",""text"":""Three""},
                {""type"":""input_value"",""name"":""Three"",""check"":""Boolean""}
            ],
            ""message0"":"" %1 %2 %3 %4 %5 %6 %7"",
            ""output"":""list_and_dictionary_TheList"",
            ""tooltip"":"""",
            ""type"":""list_and_dictionary_TheList""
        }";

        Assert.That(ClearWhiteSpaces.Replace(dictDef, ""), Is.EqualTo(ClearWhiteSpaces.Replace(dictBlock, "")));
    }

    [Test]
    public async Task Set_Value_In_Dictionary()
    {
        var script = Engine.Parser.Parse(@"
            <xml xmlns=""https://developers.google.com/blockly/xml"">
                <block type=""list_and_dictionary"" id=""p6tuv;m2|h1zv,uwR;3%"">
                    <value name=""TheList"">
                        <block type=""lists_create_with"" id=""bauQm/Al%ZaW@A~$7+Qi"">
                            <mutation items=""1""></mutation>
                            <value name=""ADD0"">
                                <block type=""list_and_dictionary_TheList"" id=""$u=_HZVP`Mw,M$KQX~}3"">
                                    <value name=""Two"">
                                        <block type=""logic_boolean"" id=""+1M;46]`hWv(Xo`vK=|D"">
                                            <field name=""BOOL"">TRUE</field>
                                        </block>
                                    </value>
                                </block>
                            </value>
                        </block>
                    </value>
                </block>
            </xml>");

        var result = (ListDictClass)(await script.Run(Site.Object))!;

        Assert.That(result.TheList, Has.Count.EqualTo(1));

        Assert.Multiple(() =>
        {
            Assert.That(result.TheList[0].ContainsKey(SampleEnum.One), Is.False);
            Assert.That(result.TheList[0][SampleEnum.Two], Is.True);
            Assert.That(result.TheList[0].ContainsKey(SampleEnum.Three), Is.False);
        });
    }
}