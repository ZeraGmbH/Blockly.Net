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
    context.EscapeMode = Fields["FLOW"] switch
    {
      "CONTINUE" => EscapeMode.Continue,
      "BREAK" => EscapeMode.Break,
      _ => throw new NotSupportedException($"{Fields["FLOW"]} flow is not supported"),
    };

    return Task.FromResult<object?>(null);
  }
}