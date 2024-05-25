

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Text;

public class TextIsEmpty : Block
{
  public override async Task<object?> Evaluate(Context context)
  {
    var text = (await Values.Evaluate("VALUE", context) ?? "").ToString();

    return string.IsNullOrEmpty(text);
  }
}