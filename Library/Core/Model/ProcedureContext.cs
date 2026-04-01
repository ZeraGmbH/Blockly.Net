namespace BlocklyNet.Core.Model;

/// <summary>
/// Execution context of a function.
/// </summary>
/// <param name="name"></param>
/// <param name="parent"></param>
public class ProcedureContext(string name, Context parent) : Context(parent)
{
    /// <summary>
    /// Name of the procedure called.
    /// </summary>
    public string Name { get; } = name;
}

