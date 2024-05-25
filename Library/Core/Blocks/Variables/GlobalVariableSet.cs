

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Variables;

// Fast-Solution
public class GlobalVariablesSet : Block
{
  public override async Task<object?> Evaluate(Context context)
  {
    var value = await Values.Evaluate("VALUE", context);
    var variableName = Fields["VAR"];

    var rootContext = context.GetRootContext();

    if (!rootContext.Variables.ContainsKey(variableName))
      rootContext.Variables.Add(variableName, value!);
    else if (value != null)
      rootContext.Variables[variableName] = value;

    return await base.Evaluate(context);
  }
}