

using System.Collections;
using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Lists;

/// <summary>
/// 
/// </summary>
public class ListsLength : Block
{
  /// <inheritdoc/>
  protected override async Task<object?> EvaluateAsync(Context context)
  {
    var count = 0;

    if (await Values.EvaluateAsync("VALUE", context) is IEnumerable value)
      foreach (var item in value)
        count++;

    return (double)count;
  }
}