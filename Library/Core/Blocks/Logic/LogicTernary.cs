

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Logic;

public class LogicTernary : Block
{
  public override async Task<object?> Evaluate(Context context)
  {
    var ifValue = await Values.Evaluate<bool>("IF", context);

    if (ifValue)
    {
      if (Values.Has("THEN"))
        return await Values.Evaluate("THEN", context);
    }
    else
    {
      if (Values.Has("ELSE"))
        return await Values.Evaluate("ELSE", context);
    }

    return null;
  }
}