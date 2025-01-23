using System.Text.Json.Nodes;

namespace BlocklyNet.Scripting.Parsing;

/// <summary>
/// 
/// </summary>
public interface IScriptParser
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="scriptAsText"></param>
    /// <returns></returns>
    IParsedScript Parse(string scriptAsText);

    /// <summary>
    /// 
    /// </summary>
    IEnumerable<JsonObject> BlockDefinitions { get; }

    /// <summary>
    /// 
    /// </summary>
    IEnumerable<JsonObject> ModelDefinitions { get; }

    /// <summary>
    /// 
    /// </summary>
    IEnumerable<Tuple<string, JsonObject>> ToolboxEntries { get; }
}
