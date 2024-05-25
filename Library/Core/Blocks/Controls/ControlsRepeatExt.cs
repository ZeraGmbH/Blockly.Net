using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Controls;

/// <summary>
/// Repeat a block for a fixed number of iterations.
/// </summary>
public class ControlsRepeatExt : Block
{
  /// <inheritdoc/>
  public override async Task<object?> Evaluate(Context context)
  {
    /* See if there is a inner block we can execute. */
    var statement = Statements.TryGet("DO");

    if (statement == null)
      return await base.Evaluate(context);

    /* Number of times to execute the inner block. */
    for (var i = await Values.Evaluate<double>("TIMES", context); i-- > 0;)
    {
      /* Execute the inner block. */
      context.Cancellation.ThrowIfCancellationRequested();

      await statement.Evaluate(context);

      try
      {
        /* See if a break of the loop is requested. */
        if (context.EscapeMode == EscapeMode.Break) break;
      }
      finally
      {
        /* Either a continue is requested or the inner block is executed normally. */
        context.EscapeMode = EscapeMode.None;
      }
    }

    /* For safety reasons reset the break mode - should not really be neccessary! */
    context.EscapeMode = EscapeMode.None;

    /* Continue with the next block. */
    return await base.Evaluate(context);
  }
}
