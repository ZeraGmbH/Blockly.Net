using BlocklyNet.Core.Model;
using BlocklyNet.Extensions;
using NUnit.Framework;

namespace BlocklyNetTests.CoreEx;

[TestFixture]
public class DelayTests : TestEnvironment
{
    /// <summary>
    /// See if the execution of a script can stall for a while.
    /// </summary>
    [Test]
    public async Task Can_Delay_Execution_Async()
    {
        /* Build block tree. */
        var block = new Delay
        {
            Values = { new() { Name = "DELAY", Block = CreateNumberBlock("500") } }
        };

        /* Run test and see if time advances for at least the given amount. */
        var start = DateTime.UtcNow;

        await block.EnterBlockAsync(new Context(Site.Object));

        Assert.That((DateTime.UtcNow - start).TotalMilliseconds, Is.GreaterThanOrEqualTo(490));
    }
}