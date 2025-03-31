

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Text;

/// <summary>
/// 
/// </summary>
public class ColourRgb : Block
{
  /// <inheritdoc/>
  protected override async Task<object?> EvaluateAsync(Context context)
  {
    var red = Convert.ToByte(await Values.EvaluateAsync("RED", context));
    var green = Convert.ToByte(await Values.EvaluateAsync("GREEN", context));
    var blue = Convert.ToByte(await Values.EvaluateAsync("BLUE", context));

    return $"#{red:x2}{green:x2}{blue:x2}";
  }
}