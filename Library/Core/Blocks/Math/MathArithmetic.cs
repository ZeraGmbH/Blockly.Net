

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Math;

public class MathArithmetic : Block
{
  public override async Task<object?> Evaluate(Context context)
  {
    var a = await Values.Evaluate<double>("A", context);
    var b = await Values.Evaluate<double>("B", context);

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