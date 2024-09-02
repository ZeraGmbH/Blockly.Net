using BlocklyNet.Core.Blocks.Text;

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
}

