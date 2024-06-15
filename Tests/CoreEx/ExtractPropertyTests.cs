using System.Text.Json;
using BlocklyNet.Core.Model;
using BlocklyNet.Extensions;
using NUnit.Framework;

namespace BlocklyNetTests.CoreEx;

[TestFixture]
public class ExtractPropertyTests : TestEnvironment
{
    public class SomeClass
    {
        public string ModelName { get; set; } = null!;

        public string Version { get; set; } = null!;
    }

    [Test]
    public async Task ExtractProperty_Parsed()
    {
        var parsed = new ParseJson
        {
            Values = { new() {
            Name = "JSON",
            Block = CreateStringBlock(
                JsonSerializer.Serialize(new
                {
                    arrayProp = new object[] { 12, "Jochen", false },
                    numberProp = 12.3d,
                }) )
        } }
        };

        var block = new ExtractProperty
        {
            Values = {
            new() { Name = "JSON", Block = parsed },
            new() { Name = "PROPERTY", Block = CreateStringBlock("numberProp") },
        }
        };

        Assert.That(await block.Evaluate(new Context(Site.Object)), Is.EqualTo(12.3d));
    }

    [Test]
    public async Task ExtractProperty_ParsedNested()
    {
        var parsed = new ParseJson
        {
            Values = { new() {
            Name = "JSON",
            Block = CreateStringBlock(
                JsonSerializer.Serialize(new
                {
                    objProp = new { self = true },
                    textProp = "text-prop",
                }) )
        } }
        };

        var block = new ExtractProperty
        {
            Values = {
            new() {
                Name = "JSON",
                Block = new ExtractProperty
                    {
                        Values = {
                            new() { Name = "JSON", Block = parsed },
                            new() { Name = "PROPERTY", Block = CreateStringBlock("objProp") },
                        }
                    } },
            new() { Name = "PROPERTY", Block = CreateStringBlock("self") },
        }
        };

        Assert.That(await block.Evaluate(new Context(Site.Object)), Is.True);
    }

    [Test]
    public async Task ExtractProperty_DotNetType()
    {
        var block = new ExtractProperty
        {
            Values = {
            new() { Name = "JSON", Block = new AnyValueBlock(new SomeClass{ ModelName = "TheModel", Version = "0.1" }) },
            new() { Name = "PROPERTY", Block = CreateStringBlock("Version") },
        }
        };

        Assert.That(await block.Evaluate(new Context(Site.Object)), Is.EqualTo("0.1"));
    }

    [Test]
    public async Task ExtractProperty_AnonymousType()
    {
        var block = new ExtractProperty
        {
            Values = {
            new() { Name = "JSON", Block = new AnyValueBlock(new {first="Jochen", last="Manns"}) },
            new() { Name = "PROPERTY", Block = CreateStringBlock("first") },
        }
        };

        Assert.That(await block.Evaluate(new Context(Site.Object)), Is.EqualTo("Jochen"));
    }
}