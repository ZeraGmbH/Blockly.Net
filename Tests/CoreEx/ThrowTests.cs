using BlocklyNet.Core.Blocks.Variables;
using BlocklyNet.Core.Model;
using BlocklyNet.Extensions;
using NUnit.Framework;

namespace BlocklyNetTests.CoreEx;

[TestFixture]
public class ThrowTests : TestEnvironment
{
    [Test]
    public async Task CanThrow_Async()
    {
        var block = new Throw { Values = { new() { Name = "MESSAGE", Block = CreateStringBlock("broken") } } };

        try
        {
            await block.EvaluateAsync(new Context(Site.Object));
        }
        catch (Exception e)
        {
            Assert.That(e.Message, Is.EqualTo("broken"));

            return;
        }

        Assert.Fail("missing exception");
    }

    [Test]
    public async Task TryCatchFinally_Try_Async()
    {
        var block = new TryCatchFinally
        {
            Statements = {
            new()
            {
                Name = "TRY",
                Block = new VariablesSet {
                    Fields = { new () { Name = "VAR", Value = "result" } },
                    Values = { new () { Name = "VALUE", Block = CreateNumberBlock("10") } },
                }
            }
        },
        };

        var context = new Context(Site.Object);

        await block.EvaluateAsync(context);

        Assert.That(context.Variables["result"], Is.EqualTo(10));
    }

    [Test]
    public async Task TryCatchFinally_Finally_Async()
    {
        var block = new TryCatchFinally
        {
            Statements = {
            new()
            {
                Name = "FINALLY",
                Block = new VariablesSet {
                    Fields = { new () { Name = "VAR", Value = "result" } },
                    Values = { new () { Name = "VALUE", Block = CreateNumberBlock("80") } },
                }
            }
        },
        };

        var context = new Context(Site.Object);

        await block.EvaluateAsync(context);

        Assert.That(context.Variables["result"], Is.EqualTo(80));
    }

    [Test]
    public async Task TryCatchFinally_Try_Finally_Async()
    {
        var block = new TryCatchFinally
        {
            Statements = {
            new()
            {
                Name = "TRY",
                Block = new VariablesSet {
                    Fields = { new () { Name = "VAR", Value = "result" } },
                    Values = { new () { Name = "VALUE", Block = CreateNumberBlock("10") } },
                }
            },
            new()
            {
                Name = "FINALLY",
                Block = new VariablesSet {
                    Fields = { new () { Name = "VAR", Value = "result" } },
                    Values = { new () { Name = "VALUE", Block = CreateNumberBlock("40") } },
                }
            }
        },
        };

        var context = new Context(Site.Object);

        await block.EvaluateAsync(context);

        Assert.That(context.Variables["result"], Is.EqualTo(40));
    }

    [Test]
    public async Task TryCatchFinally_Try_Catch_Async()
    {
        var block = new TryCatchFinally
        {
            Statements = {
            new()
            {
                Name = "TRY",
                Block = new VariablesSet {
                    Fields = { new () { Name = "VAR", Value = "result" } },
                    Values = { new () { Name = "VALUE", Block = CreateNumberBlock("10") } },
                    Next = new Throw { Values = { new () { Name="MESSAGE", Block = CreateStringBlock("bad") } } }
                }
            },
            new()
            {
                Name = "CATCH",
                Block = new VariablesSet {
                    Fields = { new () { Name = "VAR", Value = "result" } },
                    Values = { new () { Name = "VALUE", Block = CreateNumberBlock("20") } },
                }
            },
        },
        };

        var context = new Context(Site.Object);

        await block.EvaluateAsync(context);

        Assert.That(context.Variables["result"], Is.EqualTo(20));
    }

    [Test]
    public async Task TryCatchFinally_Try_Catch_Finally_Async()
    {
        var block = new TryCatchFinally
        {
            Statements = {
            new()
            {
                Name = "TRY",
                Block = new VariablesSet {
                    Fields = { new () { Name = "VAR", Value = "result" } },
                    Values = { new () { Name = "VALUE", Block = CreateNumberBlock("10") } },
                    Next = new Throw { Values = { new () { Name="MESSAGE", Block = CreateStringBlock("bad") } } }
                }
            },
            new()
            {
                Name = "CATCH",
                Block = new VariablesSet {
                    Fields = { new () { Name = "VAR", Value = "result" } },
                    Values = { new () { Name = "VALUE", Block = CreateNumberBlock("20") } },
                }
            },
            new()
            {
                Name = "FINALLY",
                Block = new VariablesSet {
                    Fields = { new () { Name = "VAR", Value = "result" } },
                    Values = { new () { Name = "VALUE", Block = CreateNumberBlock("30") } },
                }
            }
        },
        };

        var context = new Context(Site.Object);

        await block.EvaluateAsync(context);

        Assert.That(context.Variables["result"], Is.EqualTo(30));
    }

    [Test]
    public async Task TryCatchFinally_Try_Catch_Last_Error_Async()
    {
        var block = new TryCatchFinally
        {
            Statements = {
            new()
            {
                Name = "TRY",
                Block = new VariablesSet {
                    Fields = { new () { Name = "VAR", Value = "result" } },
                    Values = { new () { Name = "VALUE", Block = CreateNumberBlock("10") } },
                    Next = new Throw { Values = { new () { Name="MESSAGE", Block = CreateStringBlock("bad") } } }
                }
            },
            new()
            {
                Name = "CATCH",
                Block = new VariablesSet {
                    Fields = { new () { Name = "VAR", Value = "result" } },
                    Values = { new () { Name = "VALUE", Block = new GetLastException() } },
                }
            },
        },
        };

        var context = new Context(Site.Object);

        await block.EvaluateAsync(context);

        Assert.That(context.Variables["result"], Is.EqualTo("bad"));
    }
}