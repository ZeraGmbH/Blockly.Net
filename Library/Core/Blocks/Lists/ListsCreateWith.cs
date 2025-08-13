

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
    var list = new List<object?>();

    var count = int.Parse(Mutations.GetValue("items") ?? "0");

    for (var i = 0; i < count; i++)
    {
      context.Cancellation.ThrowIfCancellationRequested();

      var value = Values.TryGet($"ADD{i}");

      list.Add(value == null ? null : await value.EvaluateAsync(context));
    }

    return list;
  }
}