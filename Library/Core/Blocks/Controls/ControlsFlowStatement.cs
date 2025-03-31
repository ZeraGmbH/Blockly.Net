

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Controls;

/// <summary>
/// 
/// </summary>
public class ControlsFlowStatement : Block
{
  /// <inheritdoc/>
  protected override Task<object?> EvaluateAsync(Context context)
  {
    var flow = Fields["FLOW"];

    if (flow == "CONTINUE")
    {
      context.EscapeMode = EscapeMode.Continue;
      return Task.FromResult((object?)null);
    }

    if (flow == "BREAK")
    {
      context.EscapeMode = EscapeMode.Break;
      return Task.FromResult((object?)null);
    }

    throw new NotSupportedException($"{flow} flow is not supported");
  }
}