namespace BlocklyNet.Scripting.Definition;

/// <summary>
/// Default value for a script parameter.
/// </summary>
public class ScriptParameterDefaultValue
{
    /// <summary>
    /// The value to use when no actual parameter is given
    /// serialized as a JSON String.
    /// </summary>
    public string? JsonValue { get; set; }

    /// <summary>
    /// Placeholder value - ToString() of value will be used
    /// if not set.
    /// </summary>
    public string? Placeholder { get; set; }
}
