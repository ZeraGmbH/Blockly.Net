

using System.Text;
using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Text;

/// <summary>
/// 
/// </summary>
public class TextJoin : Block
{
  /// <inheritdoc/>
  public override async Task<object?> EvaluateAsync(Context context)
  {
    var items = int.Parse(Mutations.GetValue("items"));

    var sb = new StringBuilder();

    for (var i = 0; i < items; i++)
    {
      if (!Values.Has($"ADD{i}"))
        continue;

      context.Cancellation.ThrowIfCancellationRequested();

      sb.Append(await Values.EvaluateAsync($"ADD{i}", context));
    }

    return sb.ToString();
  }
}