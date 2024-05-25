

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Text;

public class ColourRgb : Block
{
  public override Task<object?> Evaluate(Context context)
  {
    var red = Convert.ToByte(Values.Evaluate("RED", context));
    var green = Convert.ToByte(Values.Evaluate("GREEN", context));
    var blue = Convert.ToByte(Values.Evaluate("BLUE", context));

    return Task.FromResult((object?)$"#{red:x2}{green:x2}{blue:x2}");
  }
}