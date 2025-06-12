using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Lists;

/// <summary>
/// List manipulation operations.
/// </summary>
public class ListsSetIndex : Block
{
    private static readonly Random rnd = new();

    /// <inheritdoc/>
    protected override async Task<object?> EvaluateAsync(Context context)
    {
        var list = new ListWrapper(await Values.EvaluateAsync("LIST", context));

        var mode = Fields["MODE"];
        var isInsert = mode switch
        {
            "INSERT" => true,
            "SET" => false,
            _ => throw new NotSupportedException($"unsupported mode ({mode})")
        };

        var where = Fields["WHERE"];
        var index = where switch
        {
            "FIRST" => 0,
            "FROM_END" => list.Count - Convert.ToInt32(await Values.EvaluateAsync("AT", context)),
            "FROM_START" => Convert.ToInt32(await Values.EvaluateAsync("AT", context)) - 1,
            "LAST" => list.Count - (isInsert ? 0 : 1),
            "RANDOM" => rnd.Next(list.Count),
            _ => throw new NotSupportedException($"unsupported where ({where})"),
        };

        var value = await Values.EvaluateAsync("TO", context);

        if (isInsert)
            list.InsertAt(index, value);
        else
            list[index] = value;

        return await base.EvaluateAsync(context);
    }
}
