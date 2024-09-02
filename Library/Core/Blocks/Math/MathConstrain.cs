

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
    var value = await Values.EvaluateAsync<double>("VALUE", context);
    var low = await Values.EvaluateAsync<double>("LOW", context);
    var high = await Values.EvaluateAsync<double>("HIGH", context);

    return System.Math.Min(System.Math.Max(value, low), high);
  }
}