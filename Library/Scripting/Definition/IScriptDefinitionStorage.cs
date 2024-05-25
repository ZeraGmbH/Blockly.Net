namespace BlocklyNet.Scripting.Definition;

/// <summary>
/// 
/// </summary>
public interface IScriptDefinitionStorage
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IQueryable<IScriptDefinition> Query();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<IScriptDefinition?> Get(string id);
}
