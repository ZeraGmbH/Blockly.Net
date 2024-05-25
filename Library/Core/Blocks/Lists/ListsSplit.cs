

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Lists;

public class ListsSplit : Block
{
  public override async Task<object?> Evaluate(Context context)
  {
    var mode = Fields["MODE"];
    var input = await Values.Evaluate<object>("INPUT", context)!;
    var delim = await Values.Evaluate<object>("DELIM", context)!;

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