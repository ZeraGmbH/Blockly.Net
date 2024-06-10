namespace BlocklyNet.Scripting.Definition;

/// <summary>
/// Describes a new script definition.
/// </summary>
public interface IScriptDefinition : IScriptDefinitionInfo
{
    /// <summary>
    /// The script code itself in the indicated serialisation.
    /// </summary>
    string Code { get; }

    /// <summary>
    /// List of parameters for the script. These will be set
    /// when the script starts.
    /// </summary>
    List<IScriptParameter> Parameters { get; }
}
