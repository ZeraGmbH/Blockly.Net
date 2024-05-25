namespace BlocklyNet.Core.Model;

/// <summary>
/// Execution context of a function.
/// </summary>
/// <param name="parent"></param>
public class ProcedureContext(Context parent) : Context(parent)
{
    /// <summary>
    /// All parameters of the function.
    /// </summary>
    public readonly IDictionary<string, object> Parameters = new Dictionary<string, object>();
}

