

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Text;

/// <summary>
/// 
/// </summary>
public class ColourRgb : Block
{
  /// <inheritdoc/>
  public override async Task<object?> Evaluate(Context context)
  {
    var red = Convert.ToByte(await Values.Evaluate("RED", context));
    var green = Convert.ToByte(await Values.Evaluate("GREEN", context));
    var blue = Convert.ToByte(await Values.Evaluate("BLUE", context));

    return $"#{red:x2}{green:x2}{blue:x2}";
  }
}