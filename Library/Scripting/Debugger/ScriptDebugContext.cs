using System.Text.Json;
using BlocklyNet.Core.Model;
using BlocklyNet.Scripting.Generic;

namespace BlocklyNet.Scripting.Debugger;

/// <summary>
/// Describes the current debug context.
/// </summary>
public class ScriptDebugContext(string scriptId, Block block, ScriptDebuggerStopReason reason, Context context, ScriptDebugger debugger) : IScriptPosition
{
    /// <summary>
    /// Current position.
    /// </summary>
    public IScriptLocation Position = new ScriptBreakpoint(scriptId, block.Id);

    /// <inheritdoc/>
    public string ScriptId { get; } = scriptId;

    /// <summary>
    /// Current block.
    /// </summary>
    public readonly Block Block = block;

    /// <inheritdoc/>
    public string BlockId { get; } = block.Id;

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
    public List<ScriptDebugVariableScope> GetVariables()
    {
        List<ScriptDebugVariableScope> list = [];

        list.AddRange(GetVariables(Context, debugger));

        return list;
    }

    /// <summary>
    /// Get variables of a context.
    /// </summary>
    /// <param name="context">Context to inspect.</param>
    /// <param name="debugger">Corresponding script debugger.</param>
    /// <returns>List of variables.</returns>
    private static List<ScriptDebugVariableScope> GetVariables(Context context, ScriptDebugger debugger)
    {
        List<ScriptDebugVariableScope> list = [];

        if (context.Engine.CurrentScript is IGenericScript script && !string.IsNullOrEmpty(script.Request.ScriptId))
            for (var current = context; current != null; current = current.Parent)
                list.Add(new()
                {
                    Context = current,
                    Debugger = debugger,
                    Procedure = current is ProcedureContext procedure ? procedure.Name : null,
                    ScriptId = script.Request.ScriptId,
                    Variables = [
                        ..current
                            .Variables
                            .Select(vi =>
                                new ScriptDebugVariableInformation
                                {
                                    Name = vi.Key,
                                    Type = current.VariableTypes.TryGetValue(vi.Key, out var type) ? type : null,
                                    Value = vi.Value == null ? null : JsonSerializer.Serialize(vi.Value, JsonUtils.JsonSettings),
                                })]
                });

        return list;
    }
}
