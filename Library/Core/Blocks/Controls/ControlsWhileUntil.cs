

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Controls;

public class ControlsWhileUntil : Block
{
  public override async Task<object?> Evaluate(Context context)
  {
    var mode = Fields["MODE"];
    var value = Values.TryGet("BOOL");

    if (!Statements.Has("DO") || null == value)
      return await base.Evaluate(context);

    var statement = Statements["DO"];

    if (mode == "WHILE")
    {
      while ((bool)(await value.Evaluate(context))!)
      {
        context.Cancellation.ThrowIfCancellationRequested();

        if (context.EscapeMode == EscapeMode.Break)
        {
          context.EscapeMode = EscapeMode.None;
          break;
        }

        await statement.Evaluate(context);
      }
    }
    else
      while (!(bool)(await value.Evaluate(context))!)
      {
        context.Cancellation.ThrowIfCancellationRequested();

        await statement.Evaluate(context);
      }

    return await base.Evaluate(context);
  }
}