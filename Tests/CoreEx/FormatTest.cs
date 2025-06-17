using BlocklyNet.Core.Model;
using BlocklyNet.Extensions;
using NUnit.Framework;

namespace BlocklyNetTests.CoreEx;

[TestFixture]
public class FormatTest : TestEnvironment
{
    /// <summary>
    /// Try to format anything as string.
    /// </summary>
    [TestCase(null, "G", "")]
    [TestCase(-5e-6, "0.000000", "-0.000005")]
    [TestCase(true, "G", "True")]
    [TestCase(13, "Liste", "Liste")]
    [TestCase("Liste", "", "Liste")]
    public async Task Can_Format_As_String_Async(object? value, string format, string expected)
    {
        var block = new FormatAsString
        {
            Values = {
                new() { Name = "VALUE", Block = new AnyValueBlock(value) } ,
                new() { Name = "FORMAT", Block = CreateStringBlock(format) }
            }
        };

        var result = await block.EnterBlockAsync(new Context(Site.Object));

        Assert.That(result, Is.EqualTo(expected));
    }

    /// <summary>
    /// Try to format list as string.
    /// </summary>
    [Test]
    public async Task Can_Format_List_As_String_Async()
    {
        var block = new FormatAsString
        {
            Values = {
                new() { Name = "VALUE", Block = new AnyValueBlock(new List<double>(){ 1, 2, 3.25 }) } ,
                new() { Name = "FORMAT", Block = CreateStringBlock("G") }
            }
        };

        var result = await block.EnterBlockAsync(new Context(Site.Object));

        Assert.That(result, Is.EqualTo("[1, 2, 3.25]"));
    }

    /// <summary>
    /// Try to format array as string.
    /// </summary>
    [Test]
    public async Task Can_Format_Array_As_String_Async()
    {
        var block = new FormatAsString
        {
            Values = {
                new() { Name = "VALUE", Block = new AnyValueBlock(new double[] { 1, 2, 3.25 }) } ,
                new() { Name = "FORMAT", Block = CreateStringBlock("G") }
            }
        };

        var result = await block.EnterBlockAsync(new Context(Site.Object));

        Assert.That(result, Is.EqualTo("[1, 2, 3.25]"));
    }
}