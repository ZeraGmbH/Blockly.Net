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
            Values = { new() { Name = "RESULT", Block = new AnyValueBlock(new GroupResult { Result = "1", Type = GroupResultType.Succeeded }) } },
            Next = new ExecutionGroup
            {
                Id = "B",
                Values = { new() { Name = "RESULT", Block = new AnyValueBlock(new GroupResult { Result = "2", Type = GroupResultType.Failed }) } }
            }
        };

        Site.Setup(s => s.BeginGroup(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

        await block.EvaluateAsync(new Context(Site.Object));

        Site.Verify(s => s.BeginGroup("A", null), Times.Once);
        Site.Verify(s => s.BeginGroup("B", null), Times.Once);
        Site.Verify(s => s.EndGroup(It.IsAny<GroupResult>()), Times.Exactly(2));
        Site.Verify(s => s.SingleStepAsync(It.IsAny<Block>()), Times.Exactly(2));
        Site.VerifyGet(s => s.Cancellation, Times.Exactly(6));
        Site.VerifyGet(s => s.CurrentScript, Times.Exactly(2));

        Site.VerifyNoOtherCalls();
    }
}