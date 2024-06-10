

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Math;

/// <summary>
/// 
/// </summary>
public class MathRandomInt : Block
{
  private static readonly Random rand = new Random();

  /// <inheritdoc/>
  public override async Task<object?> Evaluate(Context context)
  {
    var from = await Values.Evaluate<double>("FROM", context);
    var to = await Values.Evaluate<double>("TO", context);

    return (double)rand.Next((int)System.Math.Min(from, to), (int)System.Math.Max(from, to));
  }
}