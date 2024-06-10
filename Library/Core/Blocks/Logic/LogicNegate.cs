

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Logic;

/// <summary>
/// 
/// </summary>
public class LogicNegate : Block
{
  /// <inheritdoc/>
  public override async Task<object?> Evaluate(Context context)
  {
    return !(await Values.Evaluate<bool?>("BOOL", context) ?? false);
  }
}