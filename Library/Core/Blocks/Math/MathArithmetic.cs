

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Math;

/// <summary>
/// 
/// </summary>
public class MathArithmetic : Block
{
  /// <inheritdoc/>
  protected override async Task<object?> EvaluateAsync(Context context)
  {
    var a = await Values.EvaluateDoubleAsync("A", context);
    var b = await Values.EvaluateDoubleAsync("B", context);

    var opValue = Fields["OP"];

    switch (opValue)
    {
      case "MULTIPLY": return a * b;
      case "DIVIDE": return a / b;
      case "ADD": return a + b;
      case "MINUS": return a - b;
      case "POWER": return System.Math.Pow(a, b);

      default: throw new ApplicationException($"Unknown OP {opValue}");
    }
  }
}