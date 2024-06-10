

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Lists;

/// <summary>
/// 
/// </summary>
public class ListsRepeat : Block
{
  /// <inheritdoc/>
  public override async Task<object?> Evaluate(Context context)
  {
    var item = await Values.Evaluate<object>("ITEM", context);
    var num = await Values.Evaluate<double>("NUM", context);

    var list = new List<object>();

    for (var i = 0; i < num; i++)
      list.Add(item);

    return list;
  }
}