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
    Task<object?> Evaluate(Dictionary<string, object?> presets, IScriptSite engine);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="engine"></param>
    /// <returns></returns>
    Task<object?> Run(IScriptSite engine);
}
