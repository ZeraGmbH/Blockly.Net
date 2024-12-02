

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Text;

/// <summary>
/// 
/// </summary>
public class TextSubstring : Block
{
  /// <inheritdoc/>
  public override async Task<object?> EvaluateAsync(Context context)
  {
    var value = await Values.EvaluateAsync<string>("STRING", context);
    var from = Fields["WHERE1"];
    var to = Fields["WHERE2"];

    var start = from switch
    {
      "FIRST" => 1,
      "LAST" => value.Length,
      "FROM_START" => (int)await Values.EvaluateAsync<double>("AT1", context),
      "FROM_END" => value.Length - (int)await Values.EvaluateAsync<double>("AT1", context),
      _ => throw new ArgumentException($"unknown choice {from}")
    };

    var end = to switch
    {
      "FIRST" => 1,
      "LAST" => value.Length,
      "FROM_START" => (int)await Values.EvaluateAsync<double>("AT2", context),
      "FROM_END" => value.Length - (int)await Values.EvaluateAsync<double>("AT2", context),
      _ => throw new ArgumentException($"unknown choice {from}")
    };

    return value.Substring(start - 1, end - start + 1);
  }
}