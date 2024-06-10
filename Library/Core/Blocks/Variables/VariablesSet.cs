

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Variables;

/// <summary>
/// 
/// </summary>
public class VariablesSet : Block
{
  /// <inheritdoc/>
  public override async Task<object?> Evaluate(Context context)
  {
    var variables = context.Variables;
    var value = await Values.Evaluate("VALUE", context);
    var variableName = Fields["VAR"];

    // Fast-Solution
    if (variables.ContainsKey(variableName))
      variables[variableName] = value!;
    else
    {
      var rootContext = context.GetRootContext();

      if (rootContext.Variables.ContainsKey(variableName))
        rootContext.Variables[variableName] = value!;
      else
        variables.Add(variableName, value!);
    }

    return await base.Evaluate(context);
  }
}