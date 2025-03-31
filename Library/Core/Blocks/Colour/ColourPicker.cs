

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Text;

/// <summary>
/// 
/// </summary>
public class ColourPicker : Block
{
  /// <inheritdoc/>
  protected override Task<object?> EvaluateAsync(Context context)
  {
    return Task.FromResult((object?)(Fields["COLOUR"] ?? "#000000"));
  }
}