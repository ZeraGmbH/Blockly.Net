

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Math;

/// <summary>
/// 
/// </summary>
public class MathModulo : Block
{
  /// <inheritdoc/>
  public override async Task<object?> EvaluateAsync(Context context)
  {
    var dividend = await Values.EvaluateAsync<double>("DIVIDEND", context);
    var divisor = await Values.EvaluateAsync<double>("DIVISOR", context);

    return dividend % divisor;
  }
}