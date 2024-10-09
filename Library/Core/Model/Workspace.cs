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
      {
        /* Remember the result and report the last result afterwards. */
        context.Cancellation.ThrowIfCancellationRequested();

        returnValue = await block.EvaluateAsync(context);
      }

    return returnValue;
  }

  private void InspectBlockChain(Block? block)
  {
    for (; block != null; block = block.Next)
    {
      System.Diagnostics.Debug.WriteLine(block.Type);

      if (block is ExecutionGroup group)
      {
        System.Diagnostics.Debug.WriteLine(group.Fields["NAME"]);

        InspectBlockChain(group.Values.Get("RESULT")?.Block);
      }

      foreach (var statement in block.Statements)
        InspectBlockChain(statement.Block);
    }
  }

  /// <summary>
  /// Get the hierarchy of groups based on a static analysis of
  /// the script.
  /// </summary>
  /// <returns>The group information tree.</returns>
  public async Task<int> GetGroupTreeAsync()
  {
    /* Use a dummy site. */
    var context = new Context((IScriptSite)null!);

    /* Find all functions and generate the block list. */
    foreach (var block in Blocks.OfType<ProceduresDef>())
      await block.EvaluateAsync(context);

    /* Inspect all blocks. */
    foreach (var block in Blocks)
      if (block is not ProceduresDef)
        InspectBlockChain(block);

    return 0;
  }
}

