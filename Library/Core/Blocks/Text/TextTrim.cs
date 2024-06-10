

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Text;

/// <summary>
/// 
/// </summary>
public class TextTrim : Block
{
  /// <inheritdoc/>
  public override async Task<object?> Evaluate(Context context)
  {
    var mode = Fields["MODE"];

    var text = (await Values.Evaluate("TEXT", context) ?? "").ToString();

    switch (mode)
    {
      case "BOTH": return text!.Trim();
      case "LEFT": return text!.TrimStart();
      case "RIGHT": return text!.TrimEnd();
      default: throw new ApplicationException("unknown mode");
    }
  }
}