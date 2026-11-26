using BlocklyNet;
using BlocklyNet.Core;
using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;
using BlocklyNet.Scripting.Engine;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace BlocklyNetTests.Customization;

[TestFixture]
public class AbstractModelTests : TestEnvironment
{
    [JsonDerivedType(typeof(VariantA), "A")]
    [JsonDerivedType(typeof(VariantB), "B")]
    public abstract class BaseClass
    {
        public string Name { get; set; } = null!;
    }

    public class VariantA : BaseClass
    {
        public int Age { get; set; }
    }

    public class VariantB : BaseClass
    {
        public bool Male { get; set; }
    }

    public class Master
    {
        public BaseClass? SingleValue { get; set; }

        public List<BaseClass> MultiValue { get; set; } = [];
    }

    private class Configurator : IParserConfiguration
    {
        /// <inheritdoc/>
        public void Configure<TParser>(BlocklyExtensions.BlockBuilder<TParser> builder) where TParser : Parser<TParser>
        {
            builder.AddModel<VariantA>("va", "Variant A");
            builder.AddModel<VariantB>("vb", "Variant B");
            builder.AddModel<Master>("master", "Master");
        }
    }

    protected override void OnSetup(IServiceCollection services)
    {
        base.OnSetup(services);

        services.AddSingleton<IParserConfiguration, Configurator>();
    }

    [Test]
    public void Can_Reference_Other_Model()
    {
        var provider = GetService<IConfigurationService>();

        var models = provider.Configuration["models"]!.AsArray();

        models.Single(j => j!["type"]?.GetValue<string>() == "va");
        models.Single(j => j!["type"]?.GetValue<string>() == "vb");

        var master = models.Single(j => j!["type"]?.GetValue<string>() == "master")!;
        var args = (JsonArray)master["args0"]!;

        Assert.That(args, Has.Count.EqualTo(3));

        Assert.That(args[1]!.ToJsonString(), Is.EqualTo(@"{""type"":""input_value"",""name"":""SingleValue"",""check"":[""va"",""vb""]}"));
        Assert.That(args[2]!.ToJsonString(), Is.EqualTo(@"{""type"":""input_value"",""name"":""MultiValue"",""check"":""Array""}"));
    }
}

