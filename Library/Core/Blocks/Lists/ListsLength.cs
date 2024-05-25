

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Lists;

public class ListsLength : Block
{
  public override async Task<object?> Evaluate(Context context)
  {
    if (await Values.Evaluate("VALUE", context) is not IEnumerable<object> value)
      return 0.0;

    return (double)value.Count();
  }
}