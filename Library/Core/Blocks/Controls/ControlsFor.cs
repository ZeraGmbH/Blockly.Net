

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Controls;

/// <summary>
/// 
/// </summary>
public class ControlsFor : Block
{
  /// <inheritdoc/>
  protected override async Task<object?> EvaluateAsync(Context context)
  {
    var variableName = Fields["VAR"];

    var fromValue = await Values.EvaluateAsync<double>("FROM", context);
    var toValue = await Values.EvaluateAsync<double>("TO", context);
    var byValue = await Values.EvaluateAsync<double>("BY", context);

    var statement = Statements.TryGet("DO");

    context.Variables[variableName] = fromValue;

    while ((double)context.Variables[variableName]! <= toValue)
    {
      context.Cancellation.ThrowIfCancellationRequested();

      await statement!.EvaluateAsync(context);

      if (context.EscapeMode == EscapeMode.Break) break;

      context.EscapeMode = EscapeMode.None;

      context.Variables[variableName] = (double)context.Variables[variableName]! + byValue;
    }

    context.EscapeMode = EscapeMode.None;

    return await base.EvaluateAsync(context);
  }
}