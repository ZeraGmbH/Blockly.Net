

using System.Text;
using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Text;

public class TextJoin : Block
{
  public override async Task<object?> Evaluate(Context context)
  {
    var items = int.Parse(Mutations.GetValue("items"));

    var sb = new StringBuilder();

    for (var i = 0; i < items; i++)
    {
      if (!Values.Has($"ADD{i}"))
        continue;

      context.Cancellation.ThrowIfCancellationRequested();

      sb.Append(await Values.Evaluate($"ADD{i}", context));
    }

    return sb.ToString();
  }
}