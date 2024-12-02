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
    {
      /* Create the function itself and remember it. */
      context.Cancellation.ThrowIfCancellationRequested();

      await block.EvaluateAsync(context);

      functions.Add(block);
    }

    /* Process any block which is not a function. */
    foreach (var block in Blocks)
      if (!functions.Contains(block))
        for (var exec = block; exec != null; exec = exec.Next)
          if (exec.Enabled)
          {
            /* Remember the result and report the last result afterwards. */
            context.Cancellation.ThrowIfCancellationRequested();

            returnValue = await exec.EvaluateAsync(context);

            /* Did this path. */
            break;
          }

    return returnValue;
  }

  private void InspectBlockChain(Block? block, GroupInfo scope, Context context, HashSet<string> activeFunctions)
  {
    for (; block != null; block = block.Next)
    {
      var call = block is ProceduresCallNoReturn procedure ? procedure
        : block is ProceduresCallReturn function ? function
        : null;

      if (call != null)
      {
        // Some procedure call.
        var name = call.Mutations.GetValue("name");

        if (!context.Functions.TryGetValue(name, out var def)) continue;

        if (!activeFunctions.Add(name)) continue;

        foreach (var value in block.Values)
          InspectBlockChain(value.Block, scope, context, activeFunctions);

        InspectBlockChain(def.Block, scope, context, activeFunctions);

        activeFunctions.Remove(name);
      }
      else if (block is RunScript script)
      {
        // Some script call - or at least the preparation for it.
        var name = script.Values.TryGet("NAME");

        if (name?.Block is TextBlock text)
          scope.Scripts.Add(text.Fields["TEXT"]);
      }
      {
        // Regular block - maybe a nested group execution.
        var info = block is ExecutionGroup group
          ? new GroupInfo { Id = block.Id, Name = group.Fields["NAME"] }
          : scope;

        if (info != scope)
          scope.Children.Add(info);

        // Inspect all values.
        foreach (var value in block.Values)
          InspectBlockChain(value.Block, info, context, activeFunctions);

        // Inspect all statements.
        foreach (var statement in block.Statements)
          InspectBlockChain(statement.Block, info, context, activeFunctions);
      }
    }
  }

  private static void RemoveScriptNameDuplicates(IEnumerable<GroupInfo> groups)
  {
    foreach (var group in groups)
    {
      // Make sure that each name is referenced only once.
      group.Scripts = [.. group.Scripts.ToHashSet()];

      // Recurse to full tree.
      RemoveScriptNameDuplicates(group.Children);
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
    var scope = new GroupInfo();

    /* Use a dummy site. */
    var context = new Context((IScriptSite)null!);

    /* Find all functions and generate the block list. */
    foreach (var block in Blocks.OfType<ProceduresDef>())
      await block.EvaluateAsync(context);

    /* Inspect all blocks. */
    foreach (var block in Blocks)
      if (block is not ProceduresDef)
        InspectBlockChain(block, scope, context, []);

    RemoveScriptNameDuplicates(scope.Children);

    return scope.Children;
  }
}

