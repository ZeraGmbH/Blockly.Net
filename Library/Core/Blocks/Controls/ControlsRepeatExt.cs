using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Controls;

/// <summary>
/// Repeat a block for a fixed number of iterations.
/// </summary>
public class ControlsRepeatExt : Block
{
  /// <inheritdoc/>
  public override async Task<object?> EvaluateAsync(Context context)
  {
    /* See if there is a inner block we can execute. */
    var statement = Statements.TryGet("DO");

    if (statement == null)
      return await base.EvaluateAsync(context);

    /* Number of times to execute the inner block. */
    for (var i = await Values.EvaluateAsync<double>("TIMES", context); i-- > 0;)
    {
      /* Execute the inner block. */
      context.Cancellation.ThrowIfCancellationRequested();

      await statement.EvaluateAsync(context);

      /* See if a break of the loop is requested. */
      if (context.EscapeMode == EscapeMode.Break) break;

      /* Either a continue is requested or the inner block is executed normally. */
      context.EscapeMode = EscapeMode.None;
    }

    /* Reset the break mode - just in case. */
    context.EscapeMode = EscapeMode.None;

    /* Continue with the next block. */
    return await base.EvaluateAsync(context);
  }
}
