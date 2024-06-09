namespace BlocklyNet.Scripting.Definition;

/// <summary>
/// Describes a new script definition.
/// </summary>
public interface IScriptDefinition
{
    /// <summary>
    /// Unique identifier of the script.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Display name of the script.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The script code itself in the indicated serialisation.
    /// </summary>
    string Code { get; }

    /// <summary>
    /// List of parameters for the script. These will be set
    /// when the script starts.
    /// </summary>
    List<IScriptParameter> Parameters { get; }

    /// <summary>
    /// The type of the result. There are currently no further
    /// semantic restrictions - a receiver of the result has
    /// to display the data accodingly. Some of the rudimentary
    /// .NET types are supported, as for example int, double, string
    /// but also void for scripts producing no result at all.
    /// </summary>
    string? ResultType { get; }
}
