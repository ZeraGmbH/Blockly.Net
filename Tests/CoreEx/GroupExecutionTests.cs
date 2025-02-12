using BlocklyNet.Core.Model;
using BlocklyNet.Extensions;
using BlocklyNet.Scripting.Engine;
using Moq;
using NUnit.Framework;

namespace BlocklyNetTests.CoreEx;

[TestFixture]
public class GroupExecutionTests : TestEnvironment
{
    [Test]
    public async Task Can_Run_Simple_Groups_Async()
    {
        var block = new ExecutionGroup
        {
            Id = "A",
            Fields = { new() { Name = "NAME", Value = "A1" } },
            Values = { new() { Name = "RESULT", Block = new AnyValueBlock(new GroupResult { Result = "1", Type = GroupResultType.Succeeded }) } },
            Next = new ExecutionGroup
            {
                Id = "B",
                Fields = { new() { Name = "NAME", Value = "B2" } },
                Values = { new() { Name = "RESULT", Block = new AnyValueBlock(new GroupResult { Result = "2", Type = GroupResultType.Failed }) } }
            }
        };

        Site.Setup(s => s.BeginGroupAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((GroupStatus?)null);

        await block.EvaluateAsync(new Context(Site.Object));

        Site.Verify(s => s.BeginGroupAsync("A", "A1", null), Times.Once);
        Site.Verify(s => s.BeginGroupAsync("B", "B2", null), Times.Once);
        Site.Verify(s => s.EndGroupAsync(It.IsAny<GroupResult>()), Times.Exactly(2));
        Site.Verify(s => s.SingleStepAsync(It.IsAny<Block>()), Times.Exactly(2));
        Site.VerifyGet(s => s.Cancellation, Times.Exactly(6));
        Site.VerifyGet(s => s.CurrentScript, Times.Exactly(2));

        Site.VerifyNoOtherCalls();
    }
}