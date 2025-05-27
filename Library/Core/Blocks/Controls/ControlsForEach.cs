using System.Collections;
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
    var statement = Statements.TryGet("DO");

    if (statement == null) return await base.EvaluateAsync(context);

    var variableName = Fields["VAR"];

    foreach (var item in await Values.EvaluateAsync<IEnumerable>("LIST", context))
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