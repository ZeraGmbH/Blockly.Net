

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Logic;

/// <summary>
/// 
/// </summary>
public class LogicTernary : Block
{
  /// <inheritdoc/>
  public override async Task<object?> EvaluateAsync(Context context)
  {
    var ifValue = await Values.EvaluateAsync<bool>("IF", context);

    if (ifValue)
    {
      if (Values.Has("THEN"))
        return await Values.EvaluateAsync("THEN", context);
    }
    else
    {
      if (Values.Has("ELSE"))
        return await Values.EvaluateAsync("ELSE", context);
    }

    return null;
  }
}