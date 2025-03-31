

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Controls;

/// <summary>
/// 
/// </summary>
public class ControlsForEach : Block
{
  /// <inheritdoc/>
  protected override async Task<object?> EvaluateAsync(Context context)
  {
    var variableName = Fields["VAR"];
    var list = await Values.EvaluateAsync<IEnumerable<object>>("LIST", context);

    var statement = Statements.TryGet("DO");

    if (null == statement)
      return await base.EvaluateAsync(context);

    foreach (var item in list)
    {
      context.Cancellation.ThrowIfCancellationRequested();

      context.Variables[variableName] = item;

      await statement.EvaluateAsync(context);

      if (context.EscapeMode == EscapeMode.Break) break;

      context.EscapeMode = EscapeMode.None;
    }

    context.EscapeMode = EscapeMode.None;

    return await base.EvaluateAsync(context);
  }
}