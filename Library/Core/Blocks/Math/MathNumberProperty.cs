

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Math;

/// <summary>
/// 
/// </summary>
public class MathNumberProperty : Block
{
  /// <inheritdoc/>
  public override async Task<object?> EvaluateAsync(Context context)
  {
    var op = Fields["PROPERTY"];
    var number = await Values.EvaluateAsync<double>("NUMBER_TO_CHECK", context);

    switch (op)
    {
      case "EVEN": return 0 == number % 2.0;
      case "ODD": return 1 == number % 2.0;
      case "PRIME": return IsPrime((int)number);
      case "WHOLE": return 0 == number % 1.0;
      case "POSITIVE": return number > 0;
      case "NEGATIVE": return number < 0;
      case "DIVISIBLE_BY": return 0 == number % await Values.EvaluateAsync<double>("DIVISOR", context);
      default: throw new ApplicationException($"Unknown PROPERTY {op}");
    }
  }

  private static bool IsPrime(int number)
  {
    if (number == 1)
      return false;
    if (number == 2)
      return true;
    if (number % 2 == 0)
      return false;

    var boundary = (int)System.Math.Floor(System.Math.Sqrt(number));

    for (var i = 3; i <= boundary; i += 2)
    {
      if (number % i == 0)
        return false;
    }

    return true;
  }

}