using BlocklyNet.Core.Model;
using BlocklyNet.Extensions;
using NUnit.Framework;

namespace BlocklyNetTests.CoreEx;

[TestFixture]
public class NowTests : TestEnvironment
{
    /// <summary>
    /// Check the readout and formatting of the current time.
    /// </summary>
    [Test]
    public async Task Can_Get_Current_Date_And_Time_Async()
    {
        /* Build the block tree manually and run it. */
        var block = new Now
        {
            Values = { new() { Name = "FORMAT", Block = CreateStringBlock("dd.MM.yyyy") } }
        };

        var value = await block.EvaluateAsync(new Context(Site.Object));

        /* May fail when tested around midnight. */
        var now = DateTime.Now;
        var expected = $"{now.Day:00}.{now.Month:00}.{now.Year:0000}";

        Assert.That(value, Is.EqualTo(expected));
    }
}