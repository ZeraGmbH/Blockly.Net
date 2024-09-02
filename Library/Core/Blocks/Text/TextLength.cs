

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Text;

/// <summary>
/// 
/// </summary>
public class TextLength : Block
{
  /// <inheritdoc/>
  public override async Task<object?> EvaluateAsync(Context context)
  {
    var text = (await Values.EvaluateAsync("VALUE", context) ?? "").ToString();

    return (double)text!.Length;
  }
}