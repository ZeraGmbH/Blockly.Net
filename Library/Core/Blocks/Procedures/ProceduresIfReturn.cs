using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Procedures;

/// <summary>
/// 
/// </summary>
public class ProceduresIfReturn : Block
{
  /// <inheritdoc/>
  protected override async Task<object?> EvaluateAsync(Context context)
  {
    if (await Values.EvaluateAsync<bool>("CONDITION", context))
      throw new ReturnProcedureIfException(await Values.EvaluateAsync("VALUE", context));

    return await base.EvaluateAsync(context);
  }
}