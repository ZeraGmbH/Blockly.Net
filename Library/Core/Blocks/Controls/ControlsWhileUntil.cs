

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Controls;

/// <summary>
/// 
/// </summary>
public class ControlsWhileUntil : Block
{
  /// <inheritdoc/>
  public override async Task<object?> EvaluateAsync(Context context)
  {
    var mode = Fields["MODE"];
    var value = Values.TryGet("BOOL");

    if (!Statements.Has("DO") || null == value)
      return await base.EvaluateAsync(context);

    var statement = Statements["DO"];

    if (mode == "WHILE")
    {
      while ((bool)(await value.EvaluateAsync(context))!)
      {
        context.Cancellation.ThrowIfCancellationRequested();

        if (context.EscapeMode == EscapeMode.Break)
        {
          context.EscapeMode = EscapeMode.None;
          break;
        }

        await statement.EvaluateAsync(context);
      }
    }
    else
      while (!(bool)(await value.EvaluateAsync(context))!)
      {
        context.Cancellation.ThrowIfCancellationRequested();

        await statement.EvaluateAsync(context);
      }

    return await base.EvaluateAsync(context);
  }
}