namespace BlocklyNet.Scripting.Definition;

/// <summary>
/// Default value for a script parameter.
/// </summary>
public class ScriptParameterDefaultValue
{
    /// <summary>
    /// The value to use when no actual parameter is given.
    /// </summary>
    /// <value></value>
    public object? Value { get; set; }

    /// <summary>
    /// Placeholder value - ToString() of value will be used
    /// if not set.
    /// </summary>
    public string? Placeholder { get; set; }
}
