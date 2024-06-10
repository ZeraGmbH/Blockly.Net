

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Text;

/// <summary>
/// 
/// </summary>
public class ProceduresIfReturn : Block
{
  /// <inheritdoc/>
  public override async Task<object?> Evaluate(Context context)
  {
    var condition = await Values.Evaluate<bool>("CONDITION", context);

    if (condition)
      return await Values.Evaluate("VALUE", context);

    return await base.Evaluate(context);
  }
}