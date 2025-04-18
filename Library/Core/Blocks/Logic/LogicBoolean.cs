

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Logic;

/// <summary>
/// 
/// </summary>
public class LogicBoolean : Block
{
  /// <inheritdoc/>
  protected override Task<object?> EvaluateAsync(Context context)
  {
    return Task.FromResult((object?)bool.Parse(Fields["BOOL"]));
  }
}