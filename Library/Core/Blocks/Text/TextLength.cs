

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Text;

public class TextLength : Block
{
  public override async Task<object?> Evaluate(Context context)
  {
    var text = (await Values.Evaluate("VALUE", context) ?? "").ToString();

    return (double)text!.Length;
  }
}