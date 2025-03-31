

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Math;

/// <summary>
/// 
/// </summary>
public class MathRandomFloat : Block
{
  private static readonly Random rand = new Random();

  /// <inheritdoc/>
  protected override Task<object?> EvaluateAsync(Context context)
  {
    return Task.FromResult((object?)rand.NextDouble());
  }
}