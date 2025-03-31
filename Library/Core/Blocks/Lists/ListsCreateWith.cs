

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Lists;

/// <summary>
/// 
/// </summary>
public class ListsCreateWith : Block
{
  /// <inheritdoc/>
  protected override async Task<object?> EvaluateAsync(Context context)
  {
    var list = new List<object>();

    foreach (var value in Values)
    {
      context.Cancellation.ThrowIfCancellationRequested();

      list.Add((await value.EvaluateAsync(context))!);
    }

    return list;
  }
}