

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Text;

public class ProceduresIfReturn : Block
{
  public override async Task<object?> Evaluate(Context context)
  {
    var condition = await Values.Evaluate<bool>("CONDITION", context);

    if (condition)
      return await Values.Evaluate("VALUE", context);

    return await base.Evaluate(context);
  }
}