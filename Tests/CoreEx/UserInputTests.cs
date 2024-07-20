using BlocklyNet.Core.Model;
using BlocklyNet.Extensions;
using BlocklyNet.Scripting.Engine;
using Moq;
using NUnit.Framework;

namespace BlocklyNetTests.CoreEx;

[TestFixture]
public class UserInputTests : TestEnvironment
{
    private void SetupGetUserInput<T>(Mock<IScriptSite> site, object value)
        => site.Setup(s => s.GetUserInput<T>("the.key", "string")).ReturnsAsync((T?)value);

    [Test]
    public async Task UserInput()
    {
        var block = new RequestUserInput
        {
            Values = {
            new() { Name = "KEY", Block = CreateStringBlock("the.key") },
            new() { Name = "TYPE", Block = CreateStringBlock("string") },
        }
        };

        SetupGetUserInput<object?>(Site, 42);

        var input = await block.Evaluate(new Context(Site.Object));

        Assert.That(input, Is.EqualTo(42));

        Site.Verify(e => e.GetUserInput<object?>(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
}