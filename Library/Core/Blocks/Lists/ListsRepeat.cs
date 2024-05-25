

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Lists;

public class ListsRepeat : Block
{
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