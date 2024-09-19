using BlocklyNet.Core.Model;
using BlocklyNet.Extensions;
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
            Values = { new() { Name = "RESULT", Block = CreateNumberBlock("2") } },
            Next = new ExecutionGroup
            {
                Id = "B",
                Values = { new() { Name = "RESULT", Block = CreateNumberBlock("2") } }
            }
        };

        await block.EvaluateAsync(new Context(Site.Object));

        Site.Verify(s => s.BeginGroup("A"), Times.Once);
        Site.Verify(s => s.BeginGroup("B"), Times.Once);
        Site.Verify(s => s.EndGroup(It.IsAny<object?>()), Times.Exactly(2));
        Site.Verify(s => s.SingleStepAsync(It.IsAny<Block>()), Times.Exactly(2));
        Site.VerifyGet(s => s.Cancellation, Times.Exactly(6));
        Site.VerifyGet(s => s.CurrentScript, Times.Exactly(2));

        Site.VerifyNoOtherCalls();
    }
}