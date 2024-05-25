

using System.Globalization;
using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Math;

public class MathNumber : Block
{
  public override Task<object?> Evaluate(Context context)
  {
    return Task.FromResult((object?)double.Parse(Fields["NUM"], CultureInfo.InvariantCulture));
  }
}