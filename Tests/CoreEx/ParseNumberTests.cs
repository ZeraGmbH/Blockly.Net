using BlocklyNet.Core.Model;
using BlocklyNet.Extensions;
using NUnit.Framework;

namespace BlocklyNetTests.CoreEx;

[TestFixture]
public class ParseNumberTests : TestEnvironment
{
    [TestCase("13", 13)]
    [TestCase("-1.25", -1.25)]
    [TestCase("15E-1", 1.5)]
    public async Task ParseNumber_Async(string number, double expected)
    {
        var block = new ParseNumber { Values = { new() { Name = "NUMBER", Block = CreateStringBlock(number) } } };

        var result = await block.EvaluateAsync(new Context(Site.Object));

        Assert.That(result, Is.EqualTo(expected));
    }

    [TestCase("")]
    [TestCase("xx")]
    [TestCase("15E")]
    public void ParseBadNumber(string number)
    {
        var block = new ParseNumber { Values = { new() { Name = "NUMBER", Block = CreateStringBlock(number) } } };

        Assert.ThrowsAsync<FormatException>(() => block.EvaluateAsync(new Context(Site.Object)));
    }
}