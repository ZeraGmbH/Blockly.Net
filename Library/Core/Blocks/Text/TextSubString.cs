using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Text;

/// <summary>
/// 
/// </summary>
public class TextSubstring : Block
{
  /// <inheritdoc/>
  protected override async Task<object?> EvaluateAsync(Context context)
  {
    var value = await Values.EvaluateAsync<string>("STRING", context);
    var from = Fields["WHERE1"];
    var to = Fields["WHERE2"];

    var getIndex = async (string where, string at) => where switch
    {
      "FIRST" => 0,
      "LAST" => value.Length,
      "FROM_START" => (int)await Values.EvaluateAsync<double>(at, context) - 1,
      "FROM_END" => value.Length - ((int)await Values.EvaluateAsync<double>(at, context) - 1),
      _ => throw new ArgumentException($"unknown choice {where}")
    };

    return value[await getIndex(from, "AT1")..await getIndex(to, "AT2")];
  }
}