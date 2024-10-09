using System.Text.Json.Nodes;
using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;
using BlocklyNet.Scripting.Engine;
using BlocklyNet.Scripting.Parsing;

namespace BlocklyNet.Core;

/// <summary>
/// 
/// </summary>
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

      /// <inheritdoc/>
      public Task<object?> RunAsync(IScriptSite engine) => _workspace.EvaluateAsync(new Context(engine));

      /// <inheritdoc/>
      public async Task<object?> EvaluateAsync(Dictionary<string, object?> presets, IScriptSite site)
      {
        var ctx = new Context(site);

        foreach (var preset in presets)
          ctx.Variables.Add(preset);

        try
        {
          await _workspace.EvaluateAsync(ctx);
        }
        catch (ScriptStoppedEarlyException)
        {
          /* Planned abort, hopefully result is already set. */
        }

        /* If script provides a result always use this. */
        if (ctx.Variables.TryGetValue("result", out var result) && result != null) return result;

        /* If this is the root script see if group execution results are available. */
        if (site is IScriptEngine engine) return engine.CreateFlatResultFromGroups();

        /* No result at all. */
        return null;
      }

      /// <inheritdoc/>
      public Task<int> GetGroupTreeAsync() => _workspace.GetGroupTreeAsync();
    }

    public IParsedScript Parse(string scriptAsText)
    {
      Workspace workspace;
      Parser parser = _xml ? _xmlParser : _jsonParser;

      lock (parser)
        workspace = parser.Parse(scriptAsText);

      return new ParsedScript(workspace);
    }
  }

  /// <summary>
  /// 
  /// </summary>
  /// <returns></returns>
  public static XmlParser CreateXml() => new();

  /// <summary>
  /// 
  /// </summary>
  /// <returns></returns>
  public static JsonParser CreateJson() => new();

  /// <summary>
  /// 
  /// </summary>
  /// <returns></returns>
  protected readonly IDictionary<string, Func<Block>> blocks = new Dictionary<string, Func<Block>>();

  /// <summary>
  /// 
  /// </summary>
  /// <param name="xml"></param>
  /// <param name="models"></param>
  /// <param name="custom"></param>
  /// <returns></returns>
  public static IScriptParser CreateService(bool xml, IScriptModels models, IParserConfiguration? custom) => new ParserService(xml, models, custom);

  /// <summary>
  /// 
  /// </summary>
  /// <param name="script"></param>
  /// <param name="preserveWhitespace"></param>
  /// <returns></returns>
  public abstract Workspace Parse(string script, bool preserveWhitespace = false);
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TParser"></typeparam>
public abstract class Parser<TParser> : Parser where TParser : Parser
{
  /// <summary>
  /// 
  /// </summary>
  public readonly List<JsonObject> BlockDefinitions = [];

  /// <summary>
  /// 
  /// </summary>
  public readonly List<JsonObject> ModelDefinitions = [];

  /// <summary>
  /// 
  /// </summary>
  public readonly List<Tuple<string, JsonObject>> ToolboxEntries = [];

  /// <summary>
  /// 
  /// </summary>
  /// <param name="type"></param>
  /// <typeparam name="T"></typeparam>
  /// <returns></returns>
  public TParser AddBlock<T>(string type) where T : Block, new()
  {
    AddBlock(type, () => new T());

    return (this as TParser)!;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="type"></param>
  /// <param name="block"></param>
  /// <typeparam name="T"></typeparam>
  /// <returns></returns>
  public TParser AddBlock<T>(string type, T block) where T : Block
  {
    AddBlock(type, () => block);

    return (this as TParser)!;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="type"></param>
  /// <param name="blockFactory"></param>
  /// <returns></returns>
  public TParser AddBlock(string type, Func<Block> blockFactory)
  {
    if (blocks.ContainsKey(type))
      blocks[type] = blockFactory;
    else
      blocks.Add(type, blockFactory);

    return (this as TParser)!;
  }
}