using System.Text.Json;
using BlocklyNet.Core.Model;
using BlocklyNet.Scripting.Generic;

namespace BlocklyNet.Scripting.Debugger;

/// <summary>
/// Describes the current debug context.
/// </summary>
public class ScriptDebugContext(string scriptId, Block block, ScriptDebuggerStopReason reason, Context context) : IScriptPosition
{
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

        if (Context.Engine.CurrentScript is IGenericScript script && !string.IsNullOrEmpty(script.Request.ScriptId))
            for (var context = Context; context != null; context = context.Parent)
                list.Add(new()
                {
                    Context = context,
                    ScriptId = script.Request.ScriptId,
                    Variables = [
                        ..context
                            .Variables
                            .Select(vi =>
                                new ScriptDebugVariableInformation
                                {
                                    Name = vi.Key,
                                    Type = context.VariableTypes.TryGetValue(vi.Key, out var type) ? type : null,
                                    Value = vi.Value == null ? null : JsonSerializer.Serialize(vi.Value, JsonUtils.JsonSettings),
                                })]
                });

        return list;
    }
}
