using System.Dynamic;
using System.Text.Json;
using BlocklyNet.Core.Model;
using BlocklyNet.Extensions;
using NUnit.Framework;

namespace BlocklyNetTests.CoreEx;

[TestFixture]
public class ParseJsonTests : TestEnvironment
{
    [Test]
    public async Task ParseJson_Async()
    {
        var json = JsonSerializer.Serialize(new
        {
            arrayProp = new object[] { 12, "Jochen", false },
            numberProp = 12.3d,
            objProp = new { self = true },
            textProp = "text-prop",
        });

        var block = new ParseJson { Values = { new() { Name = "JSON", Block = CreateStringBlock(json) } } };

        var result = await block.EnterBlockAsync(new Context(Site.Object));

        Assert.That(result, Is.TypeOf<ExpandoObject>());

        var asJson = JsonSerializer.Serialize(result);

        Assert.That(asJson, Is.EqualTo(json));
    }
}