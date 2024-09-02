

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Controls;

/// <summary>
/// 
/// </summary>
public class ControlsForEach : Block
{
  /// <inheritdoc/>
  public override async Task<object?> EvaluateAsync(Context context)
  {
    var variableName = Fields["VAR"];
    var list = await Values.EvaluateAsync<IEnumerable<object>>("LIST", context);

    var statement = Statements.TryGet("DO");

    if (null == statement)
      return await base.EvaluateAsync(context);

    foreach (var item in list)
    {
      context.Cancellation.ThrowIfCancellationRequested();

      if (context.Variables.ContainsKey(variableName))
        context.Variables[variableName] = item;
      else
        context.Variables.Add(variableName, item);

      await statement.EvaluateAsync(context);
    }

    return await base.EvaluateAsync(context);
  }
}