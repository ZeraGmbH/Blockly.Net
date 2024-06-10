

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Logic;

/// <summary>
/// 
/// </summary>
public class LogicBoolean : Block
{
  /// <inheritdoc/>
  public override Task<object?> Evaluate(Context context)
  {
    return Task.FromResult((object?)bool.Parse(Fields["BOOL"]));
  }
}