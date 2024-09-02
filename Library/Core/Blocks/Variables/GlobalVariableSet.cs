

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Variables;

/// <summary>
/// Fast solution.
/// </summary>
public class GlobalVariablesSet : Block
{
  /// <inheritdoc/>
  public override async Task<object?> EvaluateAsync(Context context)
  {
    var value = await Values.EvaluateAsync("VALUE", context);
    var variableName = Fields["VAR"];

    var rootContext = context.GetRootContext();

    if (!rootContext.Variables.ContainsKey(variableName))
      rootContext.Variables.Add(variableName, value!);
    else if (value != null)
      rootContext.Variables[variableName] = value;

    return await base.EvaluateAsync(context);
  }
}