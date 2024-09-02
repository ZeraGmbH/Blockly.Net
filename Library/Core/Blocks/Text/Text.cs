

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Text;

/// <summary>
/// 
/// </summary>
public class TextBlock : Block
{
  /// <inheritdoc/>
  public override Task<object?> EvaluateAsync(Context context)
  {
    return Task.FromResult((object?)Fields["TEXT"]);
  }
}