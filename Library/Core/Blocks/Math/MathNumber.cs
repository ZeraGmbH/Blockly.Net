

using System.Globalization;
using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Math;

/// <summary>
/// 
/// </summary>
public class MathNumber : Block
{
  /// <inheritdoc/>
  public override Task<object?> Evaluate(Context context)
  {
    return Task.FromResult((object?)double.Parse(Fields["NUM"], CultureInfo.InvariantCulture));
  }
}