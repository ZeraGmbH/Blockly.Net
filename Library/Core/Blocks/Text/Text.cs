

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Text;

public class TextBlock : Block
{
  public override Task<object?> Evaluate(Context context)
  {
    return Task.FromResult((object?)Fields["TEXT"]);
  }
}