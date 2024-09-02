using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Lists;

/// <summary>
/// List manipulation operations.
/// </summary>
public class ListsSetIndex : Block
{
    private static readonly Random rnd = new();

    /// <inheritdoc/>
    public override async Task<object?> EvaluateAsync(Context context)
    {
        var list = new ListWrapper(await Values.EvaluateAsync("LIST", context));

        var where = Fields["WHERE"];

        var index = where switch
        {
            "FIRST" => 0,
            "FROM_END" => list.Count - Convert.ToInt32(await Values.EvaluateAsync("AT", context)),
            "FROM_START" => Convert.ToInt32(await Values.EvaluateAsync("AT", context)) - 1,
            "LAST" => list.Count - 1,
            "RANDOM" => rnd.Next(list.Count),
            _ => throw new NotSupportedException($"unsupported where ({where})"),
        };

        var value = await Values.EvaluateAsync("TO", context);

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

        return await base.EvaluateAsync(context);
    }
}
