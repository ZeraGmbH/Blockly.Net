using BlocklyNet.Core.Model;
using BlocklyNet.Extensions;
using BlocklyNet.Scripting.Generic;
using Moq;
using NUnit.Framework;

namespace BlocklyNetTests.CoreEx;

[TestFixture]
public class SetProgressTests : TestEnvironment
{
    [Test]
    public async Task SetProgress_Async()
    {
        var block = new SetProgress
        {
            Values ={
                new() { Name = "PAYLOAD", Block = CreateStringBlock(@"{""a""=1}") },
                new() { Name = "PAYLOADTYPE", Block = CreateStringBlock("text/xml") },
                new() { Name = "PROGRESS", Block = CreateNumberBlock("29.9") },
                new() { Name = "NAME", Block = CreateStringBlock("ZERA") },
                new() { Name = "ADDESTIMATION", Block = CreateBooleanBlock("TRUE") },
            }
        };

        GenericProgress? progress = null;

        Site
            .Setup(e => e.ReportProgress(It.IsAny<object>(), 0.299d, "ZERA", true, false))
            .Callback((object? p, double? rel, string? name, bool? add, bool? invisible) => progress = (GenericProgress)p!);

        await block.EnterBlockAsync(new Context(Site.Object));

        Assert.That(progress, Is.Not.Null);

        Assert.Multiple(() =>
        {
            Assert.That(progress.Payload, Is.EqualTo(@"{""a""=1}"));
            Assert.That(progress.PayloadType, Is.EqualTo("text/xml"));
            Assert.That(progress.Percentage, Is.EqualTo(29.9d));
            Assert.That(progress.ScriptId, Is.Null);
        });
    }
}