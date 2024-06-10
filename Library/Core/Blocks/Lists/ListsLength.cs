

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Lists;

/// <summary>
/// 
/// </summary>
public class ListsLength : Block
{
  /// <inheritdoc/>
  public override async Task<object?> Evaluate(Context context)
  {
    if (await Values.Evaluate("VALUE", context) is not IEnumerable<object> value)
      return 0.0;

    return (double)value.Count();
  }
}