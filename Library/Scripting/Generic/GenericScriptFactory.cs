namespace BlocklyNet.Scripting.Generic;

/// <summary>
/// 
/// </summary>
public class GenericScriptFactory : IGenericScriptFactory
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public StartGenericScript Create() => new();
}
