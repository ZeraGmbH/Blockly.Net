using BlocklyNet.Core.Blocks;
using BlocklyNet.Core.Blocks.Procedures;
using BlocklyNet.Core.Blocks.Text;
using BlocklyNet.Extensions;
using BlocklyNet.Scripting.Engine;

namespace BlocklyNet.Core.Model;

/// <summary>
/// A blocky workspace.
/// </summary>
public class Workspace : IFragment
{
  /// <summary>
  /// All blocks in the workspace.
  /// </summary>
  public readonly IList<Block> Blocks = [];

  /// <summary>
  /// Execute all blocks in the workspace.
  /// </summary>
  /// <param name="context">Execution context.</param>
  /// <returns>Result of the final block in the workspace.</returns>
  public async Task<object?> EvaluateAsync(Context context)
  {
    /* No result at all. */
    object? returnValue = null;

    /* Find all functions. */
    var functions = new List<Block>();

    foreach (var block in Blocks.OfType<ProceduresDef>())
      if (block.Enabled)
      {
        /* Create the function itself and remember it. */
        context.Cancellation.ThrowIfCancellationRequested();

        await block.EvaluateAsync(context);

        functions.Add(block);
      }

    /* Process any block which is not a function. */
    foreach (var block in Blocks)
      if (!functions.Contains(block) && block.Enabled)
      {
        /* Remember the result and report the last result afterwards. */
        context.Cancellation.ThrowIfCancellationRequested();

        returnValue = await block.EvaluateAsync(context);
      }

    return returnValue;
  }

  private void InspectBlockChain(Block? block, List<GroupInfo> groups, Context context, HashSet<string> activeFunctions)
  {
    for (; block != null; block = block.Next)
    {
      var call = block is ProceduresCallNoReturn procedure ? procedure
        : block is ProceduresCallReturn function ? function
        : null;

      if (call != null)
      {
        var name = call.Mutations.GetValue("name");

        if (!context.Functions.TryGetValue(name, out var def)) continue;

        if (!activeFunctions.Add(name)) continue;

        foreach (var value in block.Values)
          InspectBlockChain(value.Block, groups, context, activeFunctions);

        InspectBlockChain(((Statement)def).Block, groups, context, activeFunctions);

        activeFunctions.Remove(name);
      }
      else
      {
        GroupInfo? info = null;

        if (block is ExecutionGroup group)
          groups.Add(info = new GroupInfo { Id = block.Id, Name = group.Fields["NAME"] });

        var list = info?.Children ?? groups;

        foreach (var value in block.Values)
          InspectBlockChain(value.Block, list, context, activeFunctions);

        foreach (var statement in block.Statements)
          InspectBlockChain(statement.Block, list, context, activeFunctions);
      }
    }
  }

  /// <summary>
  /// Get the hierarchy of groups based on a static analysis of
  /// the script.
  /// </summary>
  /// <returns>The group information tree.</returns>
  public async Task<List<GroupInfo>> GetGroupTreeAsync()
  {
    /* Resulting list. */
    var groups = new List<GroupInfo>();

    /* Use a dummy site. */
    var context = new Context((IScriptSite)null!);

    /* Find all functions and generate the block list. */
    foreach (var block in Blocks.OfType<ProceduresDef>())
      if (block.Enabled)
        await block.EvaluateAsync(context);

    /* Inspect all blocks. */
    foreach (var block in Blocks)
      if (block is not ProceduresDef)
        InspectBlockChain(block, groups, context, []);

    return groups;
  }
}

