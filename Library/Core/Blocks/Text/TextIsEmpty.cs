

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Text;

/// <summary>
/// 
/// </summary>
public class TextIsEmpty : Block
{
  /// <inheritdoc/>
  public override async Task<object?> EvaluateAsync(Context context)
  {
    var text = (await Values.EvaluateAsync("VALUE", context) ?? "").ToString();

    return string.IsNullOrEmpty(text);
  }
}