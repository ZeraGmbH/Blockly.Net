using BlocklyNet.Core.Model;
using BlocklyNet.Extensions;
using BlocklyNet.Scripting.Engine;
using Moq;
using NUnit.Framework;

namespace BlocklyNetTests.CoreEx;

[TestFixture]
public class UserInputTests : TestEnvironment
{
    private void SetupGetUserInput<T>(Mock<IScriptSite> site, object value, double? delay = null)
    {
        if (delay == null)
        {
            site.Setup(s => s.GetUserInput<T>("the.key", "string", It.IsAny<double?>())).ReturnsAsync((T?)value);

            return;
        }

        var task = new TaskCompletionSource<object?>();
        var engine = new Mock<IScriptEngine>();

        engine.Setup(e => e.SetUserInput(null)).Callback(() => task.SetResult(null));

        site.SetupGet(s => s.Engine).Returns(engine.Object);

        site.Setup(s => s.GetUserInput<T>("the.key", "string", It.IsAny<double?>())).Returns(() => task.Task);
    }

    [TestCase(null)]
    [TestCase(10.0)]
    public async Task UserInput(double? delay)
    {
        var block = new RequestUserInput
        {
            Values = {
                new() { Name = "KEY", Block = CreateStringBlock("the.key") },
                new() { Name = "TYPE", Block = CreateStringBlock("string") },
            }
        };

        if (delay != null) block.Values.Add(new() { Name = "DELAY", Block = CreateNumberBlock($"{delay}") });

        SetupGetUserInput<object?>(Site, 42);

        var input = await block.Evaluate(new Context(Site.Object));

        Assert.That(input, Is.EqualTo(42));

        Site.Verify(e => e.GetUserInput<object?>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double?>()), Times.Once);
    }

    [Test]
    public async Task UserInputWithAutoClose()
    {
        var block = new RequestUserInput
        {
            Values = {
                new() { Name = "KEY", Block = CreateStringBlock("the.key") },
                new() { Name = "TYPE", Block = CreateStringBlock("string") },
                new() { Name = "DELAY", Block = CreateNumberBlock("1.0") },
            }
        };

        var start = DateTime.Now;

        SetupGetUserInput<object?>(Site, 43, 1.0);

        var input = await block.Evaluate(new Context(Site.Object));

        Assert.Multiple(() =>
        {
            Assert.That(input, Is.Null);
            Assert.That((DateTime.Now - start).TotalMilliseconds, Is.GreaterThanOrEqualTo(750));
        });

        Site.Verify(e => e.GetUserInput<object?>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double?>()), Times.Once);
    }

    [Test]
    public void UserInputWithAutoCloseAndException()
    {
        var block = new RequestUserInput
        {
            Values = {
                new() { Name = "KEY", Block = CreateStringBlock("the.key") },
                new() { Name = "TYPE", Block = CreateStringBlock("string") },
                new() { Name = "DELAY", Block = CreateNumberBlock("1.0") },
                new() { Name = "THROWMESSAGE", Block = CreateStringBlock("busted") },
            }
        };

        var start = DateTime.Now;

        SetupGetUserInput<object?>(Site, 43, 1.0);

        var except = Assert.ThrowsAsync<TimeoutException>(() => block.Evaluate(new Context(Site.Object)));

        Assert.That(except.Message, Is.EqualTo("busted"));
    }
}