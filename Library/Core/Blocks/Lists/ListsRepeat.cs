

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Lists;

/// <summary>
/// 
/// </summary>
public class ListsRepeat : Block
{
  /// <inheritdoc/>
  public override async Task<object?> EvaluateAsync(Context context)
  {
    var item = await Values.EvaluateAsync<object>("ITEM", context);
    var num = await Values.EvaluateAsync<double>("NUM", context);

    var list = new List<object>();

    for (var i = 0; i < num; i++)
      list.Add(item);

    return list;
  }
}