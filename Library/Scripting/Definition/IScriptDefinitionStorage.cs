namespace BlocklyNet.Scripting.Definition;

/// <summary>
/// 
/// </summary>
public interface IScriptDefinitionStorage
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<IScriptDefinition?> Get(string id);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<IScriptDefinitionInfo> Find(string name);
}
