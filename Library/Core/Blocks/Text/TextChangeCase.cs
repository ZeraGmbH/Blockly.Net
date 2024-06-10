

using System.Globalization;
using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Text;

/// <summary>
/// 
/// </summary>
public class TextCaseChange : Block
{
  /// <inheritdoc/>
  public override async Task<object?> Evaluate(Context context)
  {
    var toCase = Fields["CASE"].ToString();
    var text = (await Values.Evaluate("TEXT", context) ?? "").ToString();

    switch (toCase)
    {
      case "UPPERCASE":
        return text!.ToUpper();

      case "LOWERCASE":
        return text!.ToLower();

      case "TITLECASE":
        {
          var textInfo = new CultureInfo("en-US", false).TextInfo;
          return textInfo.ToTitleCase(text!);
        }

      default:
        throw new NotSupportedException("unknown case");

    }

  }
}