

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Math;

/// <summary>
/// 
/// </summary>
public class MathSingle : Block
{
  /// <inheritdoc/>
  public override async Task<object?> Evaluate(Context context)
  {
    var op = Fields["OP"];
    var number = await Values.Evaluate<double>("NUM", context);

    switch (op)
    {
      case "ROOT": return System.Math.Sqrt(number);
      case "ABS": return System.Math.Abs(number);
      case "NEG": return -1 * number;
      case "LN": return System.Math.Log(number);
      case "LOG10": return System.Math.Log10(number);
      case "EXP": return System.Math.Exp(number);
      case "POW10": return System.Math.Pow(number, 10);

      case "SIN": return System.Math.Sin(number / 180 * System.Math.PI);
      case "COS": return System.Math.Cos(number / 180 * System.Math.PI);
      case "TAN": return System.Math.Tan(number / 180 * System.Math.PI);
      case "ASIN": return System.Math.Asin(number / 180 * System.Math.PI);
      case "ACOS": return System.Math.Acos(number / 180 * System.Math.PI);
      case "ATAN": return System.Math.Atan(number / 180 * System.Math.PI);

      default: throw new ApplicationException($"Unknown OP {op}");
    }
  }
}