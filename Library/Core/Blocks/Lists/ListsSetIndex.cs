using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Lists;

/// <summary>
/// List manipulation operations.
/// </summary>
public class ListsSetIndex : Block
{
    private static readonly Random rnd = new();

    /// <inheritdoc/>
    public override async Task<object?> Evaluate(Context context)
    {
        var list = new ListWrapper(await Values.Evaluate("LIST", context));

        var where = Fields["WHERE"];

        var index = where switch
        {
            "FIRST" => 0,
            "FROM_END" => list.Count - Convert.ToInt32(await Values.Evaluate("AT", context)),
            "FROM_START" => Convert.ToInt32(await Values.Evaluate("AT", context)) - 1,
            "LAST" => list.Count - 1,
            "RANDOM" => rnd.Next(list.Count),
            _ => throw new NotSupportedException($"unsupported where ({where})"),
        };

        var value = await Values.Evaluate("TO", context);

        var mode = Fields["MODE"];
        switch (mode)
        {
            case "SET":
                list[index] = value;
                break;

            case "INSERT":
                list.InsertAt(index, value);
                break;

            default:
                throw new NotSupportedException($"unsupported mode ({mode})");
        }

        return await base.Evaluate(context);
    }
}
