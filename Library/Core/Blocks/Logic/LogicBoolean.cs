

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Logic;

public class LogicBoolean : Block
{
  public override Task<object?> Evaluate(Context context)
  {
    return Task.FromResult((object?)bool.Parse(Fields["BOOL"]));
  }
}