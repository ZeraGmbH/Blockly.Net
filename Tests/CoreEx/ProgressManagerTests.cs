using BlocklyNet.Scripting.Engine;
using NUnit.Framework;

namespace BlocklyNetTests.CoreEx;

[TestFixture]
public class ProgressManagerTests
{
    [TestCase(0, 0.1, 27)]
    [TestCase(0.1, 0.9, 0.375)]
    [TestCase(0.9, 0.1, null)]
    public void Calculate_Progress_Time_To_End(double initial, double next, double? expected)
    {
        var now = new DateTime(2025, 5, 25, 10, 27, 50);

        var progress = new ProgressManager { Now = () => now };

        Assert.That(progress.Latest, Is.Null);

        progress.Update(false, initial, null, true);

        Assert.That(progress.Latest, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(progress.Latest.Progress, Is.EqualTo(initial));
            Assert.That(progress.Latest.EstimatedRemainingSeconds, Is.Null);
        });

        now = now.AddSeconds(3);

        progress.Update(false, next, null, true);

        Assert.That(progress.Latest, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(progress.Latest.Progress, Is.EqualTo(next));
            Assert.That(progress.Latest.EstimatedRemainingSeconds, Is.EqualTo(expected));
        });
    }
}