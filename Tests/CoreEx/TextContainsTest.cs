using System.Reflection.Metadata.Ecma335;
using BlocklyNet.Core.Model;
using BlocklyNet.Extensions;
using BlocklyNetTests;
using NUnit.Framework;

namespace BlocklyNetTests.CoreEx;

[TestFixture]
public class TextContainsTest : TestEnvironment
{
    /// <summary>
    /// Check if string contains substring
    /// </summary>
    [TestCase("hello world", "hello", "TRUE", true)]
    [TestCase("hello world", "HELLO", "FALSE", true)]
    [TestCase("hello world", "HELLO", "TRUE", false)]
    [TestCase("hello world", "", "TRUE", true)]
    [TestCase("hello world", "foo", "TRUE", false)]
    public async Task String_Contains_Async(string originalString, string searchString, string caseSensitive, bool expected)
    {
        var block = new TextContains
        {
            Fields = {
                new() { Name = "METHOD", Value = "CONTAINS"},
                new() { Name = "CASESENSITIVE", Value = caseSensitive}
            },
            Values = {
                new() { Name = "VALUE", Block = CreateStringBlock(originalString) },
                new() { Name = "SEARCH", Block = CreateStringBlock(searchString)}
            }
        };

        var result = await block.EnterBlockAsync(new Context(Site.Object));

        Assert.That(result, Is.EqualTo(expected));
    }

    /// <summary>
    /// Check if string starts with substring
    /// </summary>
    [TestCase("hello world", "hello", "TRUE", true)]
    [TestCase("hello world", "HELLO", "FALSE", true)]
    [TestCase("hello world", "HELLO", "TRUE", false)]
    [TestCase("hello world", "", "TRUE", true)]
    [TestCase("hello world", "foo", "TRUE", false)]
    public async Task String_Starts_With_Async(string originalString, string searchString, string caseSensitive, bool expected)
    {
        var block = new TextContains
        {
            Fields = {
                new() { Name = "METHOD", Value = "STARTSWITH"},
                new() { Name = "CASESENSITIVE", Value = caseSensitive}
            },
            Values = {
                new() { Name = "VALUE", Block = CreateStringBlock(originalString) },
                new() { Name = "SEARCH", Block = CreateStringBlock(searchString)}
            }
        };

        var result = await block.EnterBlockAsync(new Context(Site.Object));

        Assert.That(result, Is.EqualTo(expected));
    }

    /// <summary>
    /// Check if string ends with substring
    /// </summary>
    [TestCase("hello world", "world", "TRUE", true)]
    [TestCase("hello world", "WORLD", "FALSE", true)]
    [TestCase("hello world", "WORLD", "TRUE", false)]
    [TestCase("hello world", "", "TRUE", true)]
    [TestCase("hello world", "foo", "TRUE", false)]
    public async Task String_Ends_With_Async(string originalString, string searchString, string caseSensitive, bool expected)
    {
        var block = new TextContains
        {
            Fields = {
                new() { Name = "METHOD", Value = "ENDSWITH"},
                new() { Name = "CASESENSITIVE", Value = caseSensitive}
            },
            Values = {
                new() { Name = "VALUE", Block = CreateStringBlock(originalString) },
                new() { Name = "SEARCH", Block = CreateStringBlock(searchString)}
            }
        };

        var result = await block.EnterBlockAsync(new Context(Site.Object));

        Assert.That(result, Is.EqualTo(expected));
    }
}