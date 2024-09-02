

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Controls;

/// <summary>
/// 
/// </summary>
public class ControlsIf : Block
{
  /// <inheritdoc/>
  public override async Task<object?> EvaluateAsync(Context context)
  {
    var ifCount = 1;
    if (null != Mutations.GetValue("elseif"))
    {
      var elseIf = Mutations.GetValue("elseif");
      ifCount = int.Parse(elseIf) + 1;
    }

    var done = false;
    for (var i = 0; i < ifCount; i++)
    {
      if (await Values.EvaluateAsync<bool>($"IF{i}", context))
      {
        context.Cancellation.ThrowIfCancellationRequested();

        await Statements[$"DO{i}"].EvaluateAsync(context);

        done = true;
        break;
      }
    }

    if (!done)
    {
      if (null != Mutations.GetValue("else"))
      {
        var elseExists = Mutations.GetValue("else");

        if (elseExists == "1")
        {
          context.Cancellation.ThrowIfCancellationRequested();

          await Statements["ELSE"].EvaluateAsync(context);
        }
      }
    }

    return await base.EvaluateAsync(context);
  }
}