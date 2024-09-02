

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Math;

/// <summary>
/// 
/// </summary>
public class MathConstant : Block
{
  /// <inheritdoc/>
  public override Task<object?> EvaluateAsync(Context context)
  {
    var constant = Fields["CONSTANT"];

    return Task.FromResult((object?)GetValue(constant));
  }

  private static double GetValue(string constant)
  {
    switch (constant)
    {
      case "PI": return System.Math.PI;
      case "E": return System.Math.E;
      case "GOLDEN_RATIO": return (1 + System.Math.Sqrt(5)) / 2;
      case "SQRT2": return System.Math.Sqrt(2);
      case "SQRT1_2": return System.Math.Sqrt(0.5);
      case "INFINITY": return double.PositiveInfinity;
      default: throw new ApplicationException($"Unknown CONSTANT {constant}");
    }
  }
}