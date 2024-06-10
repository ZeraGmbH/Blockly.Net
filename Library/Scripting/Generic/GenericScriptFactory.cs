namespace BlocklyNet.Scripting.Generic;

/// <summary>
/// 
/// </summary>
public class GenericScriptFactory : IGenericScriptFactory
{
    /// <inheritdoc/>
    public IStartGenericScript Create() => new StartGenericScript();
}
