

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Math;

/// <summary>
/// 
/// </summary>
public class MathConstrain : Block
{
  /// <inheritdoc/>
  public override async Task<object?> EvaluateAsync(Context context)
  {
    var value = await Values.EvaluateDoubleAsync("VALUE", context);
    var low = await Values.EvaluateDoubleAsync("LOW", context);
    var high = await Values.EvaluateDoubleAsync("HIGH", context);

    return System.Math.Min(System.Math.Max(value, low), high);
  }
}