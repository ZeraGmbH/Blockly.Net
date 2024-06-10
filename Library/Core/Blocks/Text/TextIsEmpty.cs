

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Text;

/// <summary>
/// 
/// </summary>
public class TextIsEmpty : Block
{
  /// <inheritdoc/>
  public override async Task<object?> Evaluate(Context context)
  {
    var text = (await Values.Evaluate("VALUE", context) ?? "").ToString();

    return string.IsNullOrEmpty(text);
  }
}