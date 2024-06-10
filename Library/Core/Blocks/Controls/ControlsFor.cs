

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Controls;

/// <summary>
/// 
/// </summary>
public class ControlsFor : Block
{
  /// <inheritdoc/>
  public override async Task<object?> Evaluate(Context context)
  {
    var variableName = Fields["VAR"];

    var fromValue = await Values.Evaluate<double>("FROM", context);
    var toValue = await Values.Evaluate<double>("TO", context);
    var byValue = await Values.Evaluate<double>("BY", context);

    var statement = Statements.TryGet("DO");

    context.Variables[variableName] = fromValue;

    while ((double)context.Variables[variableName]! <= toValue)
    {
      context.Cancellation.ThrowIfCancellationRequested();

      await statement!.Evaluate(context);

      context.Variables[variableName] = (double)context.Variables[variableName]! + byValue;
    }

    return await base.Evaluate(context);
  }
}