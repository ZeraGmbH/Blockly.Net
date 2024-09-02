

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Lists;

/// <summary>
/// 
/// </summary>
public class ListsSplit : Block
{
  /// <inheritdoc/>
  public override async Task<object?> EvaluateAsync(Context context)
  {
    var mode = Fields["MODE"];
    var input = await Values.EvaluateAsync<object>("INPUT", context)!;
    var delim = await Values.EvaluateAsync<object>("DELIM", context)!;

    switch (mode)
    {
      case "SPLIT":
        return input
            .ToString()!
            .Split(new string[] { delim.ToString()! }, StringSplitOptions.None)
            .Select(x => x as object)
            .ToList();

      case "JOIN":
        return string
            .Join(delim.ToString(), (input as IEnumerable<object>)!
            .Select(x => x.ToString()));

      default:
        throw new NotSupportedException($"unknown mode: {mode}");

    }
  }
}