using BlocklyNet.Core.Model;

namespace BlocklyNet.Scripting.Debugger;

/// <summary>
/// Describes the current debug context.
/// </summary>
public class ScriptDebugContext(string scriptId, Block block, ScriptDebuggerStopReason reason, Context context)
{
    /// <summary>
    /// Executing script normally primary key from definition in database.
    /// </summary>
    public readonly string ScriptId = scriptId;

    /// <summary>
    /// Current block.
    /// </summary>
    public readonly Block Block = block;

    /// <summary>
    /// Reason of the interception.
    /// </summary>
    public readonly ScriptDebuggerStopReason Reason = reason;

    /// <summary>
    /// Current execution context.
    /// </summary>
    public readonly Context Context = context;

    /// <summary>
    /// Retrieve all variables.
    /// </summary>
    public List<List<ScriptDebugVariableInformation>> GetVariables()
    {
        List<List<ScriptDebugVariableInformation>> list = [];

        for (var context = Context; context != null; context = context.Parent)
            list.Add([..
                context
                    .Variables
                    .Select(vi =>
                        new ScriptDebugVariableInformation
                        {
                            Name = vi.Key,
                            Type = context.VariableTypes.TryGetValue(vi.Key, out var type) ? type : null
                        })]);

        return list;
    }
}
