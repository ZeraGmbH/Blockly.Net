

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Logic;

public class LogicNull : Block
{
  public override Task<object?> Evaluate(Context context)
  {
    return Task.FromResult((object?)null);
  }
}