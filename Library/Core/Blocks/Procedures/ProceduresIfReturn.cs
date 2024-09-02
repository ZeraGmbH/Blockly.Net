

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Text;

/// <summary>
/// 
/// </summary>
public class ProceduresIfReturn : Block
{
  /// <inheritdoc/>
  public override async Task<object?> EvaluateAsync(Context context)
  {
    var condition = await Values.EvaluateAsync<bool>("CONDITION", context);

    if (condition)
      return await Values.EvaluateAsync("VALUE", context);

    return await base.EvaluateAsync(context);
  }
}