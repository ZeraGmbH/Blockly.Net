

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Lists;

/// <summary>
/// 
/// </summary>
public class ListsIsEmpty : Block
{
  /// <inheritdoc/>
  public override async Task<object?> Evaluate(Context context)
  {
    if (await Values.Evaluate("VALUE", context) is not IEnumerable<object> value)
      return true;

    return !value.Any();
  }
}