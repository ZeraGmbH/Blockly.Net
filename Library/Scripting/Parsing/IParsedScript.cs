using BlocklyNet.Core.Model;
using BlocklyNet.Scripting.Engine;

namespace BlocklyNet.Scripting.Parsing;

/// <summary>
/// 
/// </summary>
public interface IParsedScript
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="presets"></param>
    /// <param name="engine"></param>
    /// <returns></returns>
    Task<object?> EvaluateAsync(Dictionary<string, object?> presets, IScriptSite engine);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="engine"></param>
    /// <returns></returns>
    Task<object?> RunAsync(IScriptSite engine);

    /// <summary>
    /// 
    /// </summary>
    Task<List<GroupInfo>> GetGroupTreeAsync();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    string? GetVariableType(string name);

    /// <summary>
    /// 
    /// </summary>
    Block[] Blocks { get; }
}
