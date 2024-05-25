

using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Lists;

public class ListsGetIndex : Block
{
  private static readonly Random rnd = new Random();

  public override async Task<object?> Evaluate(Context context)
  {
    var values = new ListWrapper(await Values.Evaluate("VALUE", context));

    var mode = Fields["MODE"];
    var where = Fields["WHERE"];

    var index = where switch
    {
      "FIRST" => 0,
      "FROM_END" => values.Count - Convert.ToInt32(await Values.Evaluate("AT", context)),
      "FROM_START" => Convert.ToInt32(await Values.Evaluate("AT", context)) - 1,
      "LAST" => values.Count - 1,
      "RANDOM" => rnd.Next(values.Count),
      _ => throw new NotSupportedException($"unsupported where ({where})"),
    };

    switch (mode)
    {
      case "GET":
        return values[index]!;

      case "GET_REMOVE":
        var value = values[index];
        values.RemoveAt(index);
        return value!;

      case "REMOVE":
        values!.RemoveAt(index);
        return null!;

      default:
        throw new NotSupportedException($"unsupported mode ({mode})");
    }
  }
}
