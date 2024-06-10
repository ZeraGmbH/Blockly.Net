

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Text;

/// <summary>
/// 
/// </summary>
public class TextIndexOf : Block
{
  /// <inheritdoc/>
  public override async Task<object?> Evaluate(Context context)
  {
    var mode = Fields["END"];

    var text = (await Values.Evaluate("VALUE", context) ?? "").ToString();
    var term = (await Values.Evaluate("FIND", context) ?? "").ToString();

    switch (mode)
    {
      case "FIRST": return (double)text!.IndexOf(term!) + 1;
      case "LAST": return (double)text!.LastIndexOf(term!) + 1;
      default: throw new ApplicationException("unknown mode");
    }
  }
}