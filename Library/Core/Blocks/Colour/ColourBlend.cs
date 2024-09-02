

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Text;

/// <summary>
/// 
/// </summary>
public class ColourBlend : Block
{
  /// <inheritdoc/>
  public override async Task<object?> EvaluateAsync(Context context)
  {
    var colour1 = (await Values.EvaluateAsync("COLOUR1", context) ?? "").ToString();
    var colour2 = (await Values.EvaluateAsync("COLOUR2", context) ?? "").ToString();

    var ratio = System.Math.Min(System.Math.Max(await Values.EvaluateAsync<double>("RATIO", context), 0), 1);

    if (string.IsNullOrWhiteSpace(colour1) || colour1.Length != 7)
      return null!;
    if (string.IsNullOrWhiteSpace(colour2) || colour2.Length != 7)
      return null!;

    var red = (byte)(Convert.ToByte(colour1.Substring(1, 2), 16) * (1 - ratio) + (double)Convert.ToByte(colour2.Substring(1, 2), 16) * ratio);
    var green = (byte)(Convert.ToByte(colour1.Substring(3, 2), 16) * (1 - ratio) + (double)Convert.ToByte(colour2.Substring(3, 2), 16) * ratio);
    var blue = (byte)(Convert.ToByte(colour1.Substring(5, 2), 16) * (1 - ratio) + (double)Convert.ToByte(colour2.Substring(5, 2), 16) * ratio);

    return $"#{red:x2}{green:x2}{blue:x2}";
  }
}