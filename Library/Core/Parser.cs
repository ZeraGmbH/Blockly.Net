

using System.Text.Json.Nodes;
using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;
using BlocklyNet.Scripting.Engine;
using BlocklyNet.Scripting.Parsing;

namespace BlocklyNet.Core;

public abstract class Parser
{
  private class ParserService(bool _xml, IScriptModels models, IParserConfiguration? custom) : IScriptParser
  {
    private readonly XmlParser _xmlParser = CreateXml().AddCustomBlocks(models, builder => custom?.Configure(builder));

    private readonly JsonParser _jsonParser = CreateJson().AddCustomBlocks(models, builder => custom?.Configure(builder));

    public IEnumerable<JsonObject> BlockDefinitions => _xml ? _xmlParser.BlockDefinitions : _jsonParser.BlockDefinitions;

    public IEnumerable<JsonObject> ModelDefinitions => _xml ? _xmlParser.ModelDefinitions : _jsonParser.ModelDefinitions;

    public IEnumerable<Tuple<string, JsonObject>> ToolboxEntries => _xml ? _xmlParser.ToolboxEntries : _jsonParser.ToolboxEntries;

    private class ParsedScript(Workspace workspace) : IParsedScript
    {
      private readonly Workspace _workspace = workspace;

      public Task<object?> Run(IScriptSite engine) => _workspace.Evaluate(new Context(engine));

      public async Task<object?> Evaluate(Dictionary<string, object?> presets, IScriptSite engine)
      {
        var ctx = new Context(engine);

        foreach (var preset in presets)
          ctx.Variables.Add(preset);

        try
        {
          await _workspace.Evaluate(ctx);
        }
        catch (ScriptStoppedEarlyException)
        {
          /* Planned abort, hopefully result is already set. */
        }

        return ctx.Variables.TryGetValue("result", out var result) ? result : null;
      }
    }

    public IParsedScript Parse(string scriptAsText)
    {
      Parser parser = _xml ? _xmlParser : _jsonParser;

      lock (parser)
        return new ParsedScript(parser.Parse(scriptAsText));
    }
  }

  public static XmlParser CreateXml() => new();

  public static JsonParser CreateJson() => new();

  protected readonly IDictionary<string, Func<Block>> blocks = new Dictionary<string, Func<Block>>();

  public static IScriptParser CreateService(bool xml, IScriptModels models, IParserConfiguration? custom) => new ParserService(xml, models, custom);

  public abstract Workspace Parse(string script, bool preserveWhitespace = false);
}

public abstract class Parser<TParser> : Parser where TParser : Parser
{
  public readonly List<JsonObject> BlockDefinitions = [];

  public readonly List<JsonObject> ModelDefinitions = [];

  public readonly List<Tuple<string, JsonObject>> ToolboxEntries = [];

  public TParser AddBlock<T>(string type) where T : Block, new()
  {
    AddBlock(type, () => new T());

    return (this as TParser)!;
  }

  public TParser AddBlock<T>(string type, T block) where T : Block
  {
    AddBlock(type, () => block);

    return (this as TParser)!;
  }

  public TParser AddBlock(string type, Func<Block> blockFactory)
  {
    if (blocks.ContainsKey(type))
      blocks[type] = blockFactory;
    else
      blocks.Add(type, blockFactory);

    return (this as TParser)!;
  }
}