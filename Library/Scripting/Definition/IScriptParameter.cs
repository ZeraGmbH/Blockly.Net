namespace BlocklyNet.Scripting.Definition;

/// <summary>
/// Parameter to a script a use can set before 
/// starting the script.
/// </summary>
public interface IScriptParameter
{
    /// <summary>
    /// The type of the input.
    /// </summary>
    string Type { get; }

    /// <summary>
    /// The name of the parameter.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Set to indicate that the parameter is required.
    /// </summary>
    public bool? Required { get; }
}