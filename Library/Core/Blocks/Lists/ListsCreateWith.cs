

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Lists;

/// <summary>
/// 
/// </summary>
public class ListsCreateWith : Block
{
  /// <inheritdoc/>
  public override async Task<object?> Evaluate(Context context)
  {
    var list = new List<object>();

    foreach (var value in Values)
    {
      context.Cancellation.ThrowIfCancellationRequested();

      list.Add((await value.Evaluate(context))!);
    }

    return list;
  }
}