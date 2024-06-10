

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Math;

/// <summary>
/// 
/// </summary>
public class MathConstrain : Block
{
  /// <inheritdoc/>
  public override async Task<object?> Evaluate(Context context)
  {
    var value = await Values.Evaluate<double>("VALUE", context);
    var low = await Values.Evaluate<double>("LOW", context);
    var high = await Values.Evaluate<double>("HIGH", context);

    return System.Math.Min(System.Math.Max(value, low), high);
  }
}