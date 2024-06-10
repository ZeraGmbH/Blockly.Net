

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Text;

/// <summary>
/// 
/// </summary>
public class ColourPicker : Block
{
  /// <inheritdoc/>
  public override Task<object?> Evaluate(Context context)
  {
    return Task.FromResult((object?)(Fields["COLOUR"] ?? "#000000"));
  }
}