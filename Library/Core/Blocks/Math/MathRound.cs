

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Math;

/// <summary>
/// 
/// </summary>
public class MathRound : Block
{
  /// <inheritdoc/>
  public override async Task<object?> EvaluateAsync(Context context)
  {
    var op = Fields["OP"];
    var number = await Values.EvaluateAsync<double>("NUM", context);

    switch (op)
    {
      case "ROUND": return System.Math.Round(number);
      case "ROUNDUP": return System.Math.Ceiling(number);
      case "ROUNDDOWN": return System.Math.Floor(number);
      default: throw new ApplicationException($"Unknown OP {op}");
    }
  }
}