

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Lists;

public class ListsIsEmpty : Block
{
  public override async Task<object?> Evaluate(Context context)
  {
    if (await Values.Evaluate("VALUE", context) is not IEnumerable<object> value)
      return true;

    return !value.Any();
  }
}