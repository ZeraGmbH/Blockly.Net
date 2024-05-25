

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Text;

public class ColourPicker : Block
{
  public override Task<object?> Evaluate(Context context)
  {
    return Task.FromResult((object?)(Fields["COLOUR"] ?? "#000000"));
  }
}