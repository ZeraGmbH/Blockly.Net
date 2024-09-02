

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Logic;

/// <summary>
/// 
/// </summary>
public class LogicNegate : Block
{
  /// <inheritdoc/>
  public override async Task<object?> EvaluateAsync(Context context)
  {
    return !(await Values.EvaluateAsync<bool?>("BOOL", context) ?? false);
  }
}