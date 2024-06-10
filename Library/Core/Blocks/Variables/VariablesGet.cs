

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Variables;

/// <summary>
/// 
/// </summary>
public class VariablesGet : Block
{
  /// <inheritdoc/>
  public override Task<object?> Evaluate(Context context)
  {
    var variableName = Fields["VAR"];

    // Fast-Solution
    if (!context.Variables.ContainsKey(variableName))
    {
      if (!context.GetRootContext().Variables.ContainsKey(variableName))
        return Task.FromResult((object?)null);

      return Task.FromResult((object?)context.GetRootContext().Variables[variableName]);
    }

    return Task.FromResult((object?)context.Variables[variableName]);
  }
}