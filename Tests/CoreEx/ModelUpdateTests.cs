using BlocklyNet.Core.Model;
using BlocklyNet.Extensions;
using NUnit.Framework;

namespace BlocklyNetTests.CoreEx;

[TestFixture]
public class ModelUpdateTests : TestEnvironment
{
    enum Modes { One, Two };

    private Context Context = null!;

    protected override void OnStartup()
    {
        Context = new Context(Site.Object);

        base.OnStartup();
    }

    public class InnerClass
    {
        public double Id { get; set; }
    }

    public class OuterClass
    {
        public string Name { get; set; } = null!;

        public List<InnerClass> Inner { get; set; } = [];
    }

    [Test]
    public async Task Can_Update_Class_Instance()
    {
        var instance = new OuterClass
        {
            Name = "Outer",
            Inner = { new() { Id = -1 }, new() { Id = -2 }, new() { Id = -3 } }
        };

        Context.Variables["instance"] = instance;

        var block = new UpdateModelProperty
        {
            Fields = { new() { Name = "VAR", Value = "instance" } },
            Values = {
                new() { Name = "VALUE", Block = CreateStringBlock("Replace-Outer") },
                new() { Name = "PATH", Block = CreateStringBlock("Name") },
            }
        };

        await block.Evaluate(Context);

        Assert.That(instance.Name, Is.EqualTo("Replace-Outer"));

        for (var i = 0; i < instance.Inner.Count; i++)
        {
            var test = new UpdateModelProperty
            {
                Fields = { new() { Name = "VAR", Value = "instance" } },
                Values = {
                    new() { Name = "VALUE", Block = CreateNumberBlock($"{i + 42}") },
                    new() { Name = "PATH", Block = CreateStringBlock("Inner[].Id") },
                    new() { Name = "INDEXES", Block = new AnyValueBlock(new [] { i + 1 } ) },
                }
            };

            await test.Evaluate(Context);

            Assert.That(instance.Inner[i].Id, Is.EqualTo(42 + i));
        }
    }

    private static string jsonModel = @"{
        ""name"": ""Outer"",
        ""inner"": [
            { ""id"": -1 },
            { ""id"": -2 },
            { ""id"": -3 }
        ]
    }";

    [Test]
    public async Task Can_Update_Parsed_Json()
    {
        var parser = new ParseJson { Values = { new() { Name = "JSON", Block = CreateStringBlock(jsonModel) } } };

        dynamic instance = (await parser.Evaluate(Context))!;

        Context.Variables["instance"] = instance;

        var block = new UpdateModelProperty
        {
            Fields = { new() { Name = "VAR", Value = "instance" } },
            Values = {
            new() { Name = "VALUE", Block = CreateStringBlock("Replace-Outer") },
            new() { Name = "PATH", Block = CreateStringBlock("name") },
        }
        };

        await block.Evaluate(Context);

        Assert.That(instance.name, Is.EqualTo("Replace-Outer"));

        for (var i = 0; i < 3; i++)
        {
            var test = new UpdateModelProperty
            {
                Fields = { new() { Name = "VAR", Value = "instance" } },
                Values = {
                new() { Name = "VALUE", Block = CreateNumberBlock($"{i + 42}") },
                new() { Name = "PATH", Block = CreateStringBlock("inner[].id") },
                new() { Name = "INDEXES", Block = new AnyValueBlock(new [] { i + 1 } ) },
            }
            };

            await test.Evaluate(Context);

            Assert.That(instance.inner[i].id, Is.EqualTo(42 + i));
        }
    }

    [Test]
    public async Task Can_Update_Untyped_Dictionary()
    {
        var instance = new Dictionary<string, double> { { "a", -1 }, { "b", -2 } };

        Context.Variables["instance"] = instance;

        var block = new UpdateModelProperty
        {
            Fields = { new() { Name = "VAR", Value = "instance" } },
            Values = {
            new() { Name = "VALUE", Block = CreateNumberBlock("42") },
            new() { Name = "PATH", Block = CreateStringBlock("b") },
        }
        };

        await block.Evaluate(Context);

        Assert.That(instance["b"], Is.EqualTo(42));
    }

    [Test]
    public async Task Can_Update_Typed_Dictionary()
    {
        var instance = new Dictionary<Modes, double> { { Modes.One, -1 }, { Modes.Two, -2 } };

        Context.Variables["instance"] = instance;

        var block = new UpdateModelProperty
        {
            Fields = { new() { Name = "VAR", Value = "instance" } },
            Values = {
            new() { Name = "VALUE", Block = CreateNumberBlock("42") },
            new() { Name = "PATH", Block = CreateStringBlock(Modes.One.ToString()) },
        }
        };

        await block.Evaluate(Context);

        Assert.That(instance[Modes.One], Is.EqualTo(42));
    }

    [Test]
    public async Task Can_Read_Class_Instance()
    {

        Context.Variables["instance"] = new OuterClass
        {
            Name = "Outer",
            Inner = { new() { Id = -1 }, new() { Id = -2 }, new() { Id = -3 } }
        };

        var block = new ReadFromModel
        {
            Fields = { new() { Name = "VAR", Value = "instance" } },
            Values = { new() { Name = "PATH", Block = CreateStringBlock("Name") }, }
        };

        Assert.That(await block.Evaluate(Context), Is.EqualTo("Outer"));

        for (var i = 0; i < 3; i++)
        {
            var test = new ReadFromModel
            {
                Fields = { new() { Name = "VAR", Value = "instance" } },
                Values = {
                new() { Name = "PATH", Block = CreateStringBlock("Inner[].Id") },
                new() { Name = "INDEXES", Block = new AnyValueBlock(new [] { i + 1 } ) },
            }
            };

            Assert.That(await test.Evaluate(Context), Is.EqualTo(-(i + 1)));
        }
    }

    [Test]
    public async Task Can_Read_Parsed_Json()
    {
        var parser = new ParseJson { Values = { new() { Name = "JSON", Block = CreateStringBlock(jsonModel) } } };

        Context.Variables["instance"] = await parser.Evaluate(Context);

        var block = new ReadFromModel
        {
            Fields = { new() { Name = "VAR", Value = "instance" } },
            Values = { new() { Name = "PATH", Block = CreateStringBlock("name") }, }
        };

        Assert.That(await block.Evaluate(Context), Is.EqualTo("Outer"));

        for (var i = 0; i < 3; i++)
        {
            var test = new ReadFromModel
            {
                Fields = { new() { Name = "VAR", Value = "instance" } },
                Values = {
                new() { Name = "PATH", Block = CreateStringBlock("inner[].id") },
                new() { Name = "INDEXES", Block = new AnyValueBlock(new [] { i + 1 } ) },
            }
            };

            Assert.That(await test.Evaluate(Context), Is.EqualTo(-(i + 1)));
        }
    }

    [Test]
    public async Task Can_Read_Untyped_Dictionary()
    {
        Context.Variables["instance"] = new Dictionary<string, int> { { "a", -1 }, { "b", -2 } };

        var block = new ReadFromModel
        {
            Fields = { new() { Name = "VAR", Value = "instance" } },
            Values = { new() { Name = "PATH", Block = CreateStringBlock("b") }, }
        };

        Assert.That(await block.Evaluate(Context), Is.EqualTo(-2));
    }

    [Test]
    public async Task Can_Read_Typed_Dictionary()
    {
        Context.Variables["instance"] = new Dictionary<Modes, int> { { Modes.One, -1 }, { Modes.Two, -2 } };

        var block = new ReadFromModel
        {
            Fields = { new() { Name = "VAR", Value = "instance" } },
            Values = { new() { Name = "PATH", Block = CreateStringBlock(Modes.One.ToString()) }, }
        };

        Assert.That(await block.Evaluate(Context), Is.EqualTo(-1));
    }
}