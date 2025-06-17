

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Lists;

/// <summary>
/// 
/// </summary>
public class ListsIndexOf : Block
{
  /// <inheritdoc/>
  protected override async Task<object?> EvaluateAsync(Context context)
  {
    var value = await Values.EvaluateAsync<IEnumerable<object>>("VALUE", context);
    var find = await Values.EvaluateAsync("FIND", context);

    return (double)(Fields["END"] switch
    {
      "FIRST" => value.ToList().IndexOf(find!) + 1,
      "LAST" => value.ToList().LastIndexOf(find!) + 1,
      _ => throw new NotSupportedException($"Unknown end: '{Fields["END"]}'"),
    });
  }
}