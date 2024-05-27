namespace BlocklyNet.Scripting.Generic;

public class GenericScriptFactory : IGenericScriptFactory
{
    public StartGenericScript Create() => new();
}
