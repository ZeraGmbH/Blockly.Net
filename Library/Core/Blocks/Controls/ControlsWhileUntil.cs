using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Controls;

/// <summary>
/// 
/// </summary>
public class ControlsWhileUntil : Block
{
  /// <inheritdoc/>
  protected override async Task<object?> EvaluateAsync(Context context)
  {
    var value = Values.TryGet("BOOL");
    var statement = Statements.TryGet("DO");

    if (statement == null || null == value)
      return await base.EvaluateAsync(context);

    var execStatementAndTestForBreak = async () =>
    {
      context.Cancellation.ThrowIfCancellationRequested();

      await statement.EvaluateAsync(context);

      if (context.EscapeMode == EscapeMode.Break) return true;

      context.EscapeMode = EscapeMode.None;

      return false;
    };

    switch (Fields["MODE"])
    {
      case "WHILE":
        while ((bool)(await value.EvaluateAsync(context))!)
          if (await execStatementAndTestForBreak())
            break;

        break;
      case "UNTIL":
        do
          if (await execStatementAndTestForBreak())
            break;
        while (!(bool)(await value.EvaluateAsync(context))!);

        break;
    }

    context.EscapeMode = EscapeMode.None;

    return await base.EvaluateAsync(context);
  }
}