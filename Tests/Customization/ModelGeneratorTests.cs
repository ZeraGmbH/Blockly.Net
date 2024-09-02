using BlocklyNet;
using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;
using BlocklyNet.Scripting.Engine;
using Moq;
using NUnit.Framework;
using System.Text.Json;

namespace BlocklyNetTests.Customization;

[TestFixture]
public class ModelGeneratorTests
{
    public class TestModel
    {
        public string StringProp { get; set; } = null!;
        public string? OptionalStringProp { get; set; }
        public bool BoolProp { get; set; }
        public bool? OptionalBoolProp { get; set; }
        public int IntProp { get; set; }
        public int? OptionalIntProp { get; set; }
        public double DoubleProp { get; set; }
        public double? OptionalDoubleProp { get; set; }
    }

    public class InnerRef
    {
        public bool InnerProp { get; set; }
    }

    public class OuterRef
    {
        public bool OuterProp { get; set; }
        public InnerRef Inner { get; set; } = null!;

    }

    public enum EnumModel
    {
        A,
        B
    }

    public class EnumRef
    {
        public bool Prop { get; set; }

        public EnumModel Value { get; set; }
    }

    public class ArrayRef
    {
        public List<bool> Props { get; set; } = [];

        public List<InnerRef> Values { get; set; } = [];
    }

    private class ObjectValue
    {
        public object SingleRequired { get; set; } = null!;

        public object? Single { get; set; }

        public List<object> MultipleRequired { get; set; } = [];

        public List<object?> Multiple { get; set; } = [];
    }

    private class ConstantBlock(object? value) : Block
    {
        private readonly object? _value = value;

        public override Task<object?> EvaluateAsync(Context context) => Task.FromResult(_value);
    }

    [Test]
    public void Can_Reference_Other_Model()
    {
        var models = new ModelCache();

        models.Add<InnerRef>("inner");

        var outer = ModelBlock<OuterRef>.Initialize("outer", "OUTER", models, (type, key, name) => false);

        var blockJson = JsonSerializer.Serialize(outer.Item1, JsonUtils.JsonSettings);
        var toolJson = JsonSerializer.Serialize(outer.Item2, JsonUtils.JsonSettings);

        Assert.Multiple(() =>
        {
            Assert.That(blockJson, Has.Length.EqualTo(372));
            Assert.That(toolJson, Has.Length.EqualTo(132));
        });
    }

    [Test]
    public void Can_Use_Enum()
    {
        var models = new ModelCache();

        models.Add<EnumModel>("enum_model");

        var emodel = EnumBlock<EnumModel>.Initialize("enum_model", "ENUMMODEL");
        var model = ModelBlock<EnumRef>.Initialize("enum", "ENUM", models, (type, key, name) => false);

        var blockJson = JsonSerializer.Serialize(model.Item1, JsonUtils.JsonSettings);
        var toolJson = JsonSerializer.Serialize(model.Item2, JsonUtils.JsonSettings);

        Assert.Multiple(() =>
        {
            Assert.That(blockJson, Has.Length.EqualTo(358));
            Assert.That(toolJson, Has.Length.EqualTo(189));
        });
    }

    [Test]
    public void Can_Use_Arrays()
    {
        var models = new ModelCache();

        models.Add<InnerRef>("inner");

        var outer = ModelBlock<ArrayRef>.Initialize("array", "ARRAY", models, (type, key, name) => false);

        var blockJson = JsonSerializer.Serialize(outer.Item1, JsonUtils.JsonSettings);
        var toolJson = JsonSerializer.Serialize(outer.Item2, JsonUtils.JsonSettings);

        Assert.Multiple(() =>
        {
            Assert.That(blockJson, Has.Length.EqualTo(397));
            Assert.That(toolJson, Has.Length.EqualTo(59));
        });
    }

    [Test]
    public async Task Can_Create_Blockly_Model_Dynamically_Async()
    {
        var definitions = ModelBlock<TestModel>.Initialize("K-2", "N-3", new ModelCache(), (type, key, name) => false);
        var blockJson = JsonSerializer.Serialize(definitions.Item1, JsonUtils.JsonSettings);
        var toolJson = JsonSerializer.Serialize(definitions.Item2, JsonUtils.JsonSettings);

        Assert.Multiple(() =>
        {
            Assert.That(blockJson, Has.Length.EqualTo(1310));
            Assert.That(toolJson, Has.Length.EqualTo(620));
        });

        var model = new ModelBlock<TestModel>();

        model.Values.Add(new() { Name = nameof(TestModel.BoolProp), Block = new ConstantBlock(true) });
        model.Values.Add(new() { Name = nameof(TestModel.DoubleProp), Block = new ConstantBlock(Math.PI) });
        model.Values.Add(new() { Name = nameof(TestModel.IntProp), Block = new ConstantBlock(29) });
        model.Values.Add(new() { Name = nameof(TestModel.OptionalBoolProp), Block = new ConstantBlock(true) });
        model.Values.Add(new() { Name = nameof(TestModel.OptionalDoubleProp), Block = new ConstantBlock(Math.E) });
        model.Values.Add(new() { Name = nameof(TestModel.OptionalIntProp), Block = new ConstantBlock(9) });
        model.Values.Add(new() { Name = nameof(TestModel.OptionalStringProp), Block = new ConstantBlock("optionalString") });
        model.Values.Add(new() { Name = nameof(TestModel.StringProp), Block = new ConstantBlock("testString") });

        var siteMock = new Mock<IScriptSite>();
        var result = await model.EvaluateAsync(new(siteMock.Object));

        Assert.That(result, Is.InstanceOf<TestModel>());

        var testModel = (TestModel)result;

        Assert.Multiple(() =>
        {
            Assert.That(testModel.BoolProp, Is.EqualTo(true));
            Assert.That(testModel.DoubleProp, Is.EqualTo(Math.PI));
            Assert.That(testModel.IntProp, Is.EqualTo(29));
            Assert.That(testModel.OptionalBoolProp, Is.EqualTo(true));
            Assert.That(testModel.OptionalDoubleProp, Is.EqualTo(Math.E));
            Assert.That(testModel.OptionalIntProp, Is.EqualTo(9));
            Assert.That(testModel.OptionalStringProp, Is.EqualTo("optionalString"));
            Assert.That(testModel.StringProp, Is.EqualTo("testString"));
        });
    }

    [Test]
    public void Can_Use_Object_Data_Type()
    {
        var models = new ModelCache();

        models.Add<ObjectValue>("withObject");

        var outer = ModelBlock<ObjectValue>.Initialize("obj", "OBJ", models, (type, key, name) => false);

        var blockJson = JsonSerializer.Serialize(outer.Item1, JsonUtils.JsonSettings);
        var toolJson = JsonSerializer.Serialize(outer.Item2, JsonUtils.JsonSettings);

        Assert.Multiple(() =>
        {
            Assert.That(blockJson, Has.Length.EqualTo(642));
            Assert.That(toolJson, Has.Length.EqualTo(55));
        });
    }
}

