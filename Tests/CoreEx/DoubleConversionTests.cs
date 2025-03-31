using BlocklyNet.Core.Blocks.Math;
using BlocklyNet.Core.Model;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BlocklyNetTests.CoreEx;

[TestFixture]
public class DoubleConversionTests : TestEnvironment
{
    class CustomNumber(double value)
    {
        public readonly double Value = value;
    }

    class CustomNumberExtractor : IDoubleExtractor
    {
        public double GetNumber(object value) => ((CustomNumber)value).Value;
    }

    protected override void OnSetup(IServiceCollection services)
    {
        services.AddTransient<IDoubleExtractor, CustomNumberExtractor>();

        base.OnSetup(services);
    }

    /// <summary>
    /// See if the execution of a script can stall for a while.
    /// </summary>
    [Test]
    public async Task Can_Convert_Custom_Numbers_Async()
    {
        for (var left = 2; left-- > 0;)
            for (var right = 2; right-- > 0;)
            {
                /* Build block tree. */
                var block = new MathArithmetic
                {
                    Fields = { new() { Name = "OP", Value = "ADD" } },
                    Values = {
                        new() { Name = "A", Block = left == 1 ? new AnyValueBlock(new CustomNumber(5d)) : new AnyValueBlock(5d) },
                        new() { Name = "B", Block = right == 1 ? new AnyValueBlock(new CustomNumber(7d)) : new AnyValueBlock(7d) },
                    }
                };

                var sum = await block.EvaluateAsync(new Context(Site.Object));

                Assert.That((double)sum!, Is.EqualTo(12));
            }
    }
}