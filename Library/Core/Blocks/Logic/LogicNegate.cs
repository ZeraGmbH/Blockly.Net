

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Logic;

public class LogicNegate : Block
{
  public override async Task<object?> Evaluate(Context context)
  {
    return !(await Values.Evaluate<bool?>("BOOL", context) ?? false);
  }
}