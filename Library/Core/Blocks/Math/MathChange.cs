

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Math;

public class MathChange : Block
{
  public override async Task<object?> Evaluate(Context context)
  {
    var variableName = Fields["VAR"];
    var delta = await Values.Evaluate<double>("DELTA", context);

    if (!context.Variables.ContainsKey(variableName))
      throw new ApplicationException($"variable {variableName} not declared");

    var value = (double)context.Variables[variableName]!;
    value += delta;
    context.Variables[variableName] = value;

    return await base.Evaluate(context);
  }
}