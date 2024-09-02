

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Logic;

/// <summary>
/// 
/// </summary>
public class LogicNull : Block
{
  /// <inheritdoc/>
  public override Task<object?> EvaluateAsync(Context context)
  {
    return Task.FromResult((object?)null);
  }
}