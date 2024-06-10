

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Lists;

/// <summary>
/// 
/// </summary>
public class ListsIndexOf : Block
{
  /// <inheritdoc/>
  public override async Task<object?> Evaluate(Context context)
  {
    var direction = Fields["END"];
    var value = await Values.Evaluate<IEnumerable<object>>("VALUE", context);
    var find = await Values.Evaluate("FIND", context);

    switch (direction)
    {
      case "FIRST":
        return value.ToList().IndexOf(find!) + 1;

      case "LAST":
        return value.ToList().LastIndexOf(find!) + 1;

      default:
        throw new NotSupportedException("$Unknown end: {direction}");
    }
  }
}