using BlocklyNet.Core.Model;

namespace BlocklyNet.Core.Blocks.Text;


/// <summary>
/// Block for replacing one string with another in the text.
/// </summary>
public class TextReplace : Block
{
    /// <inheritdoc/>
    protected override async Task<object?> EvaluateAsync(Context context)
    {
        var text = await Values.EvaluateAsync<string>("TEXT", context);
        var from = await Values.EvaluateAsync<string>("FROM", context);
        var to = await Values.EvaluateAsync<string>("TO", context);

        return text.Replace(from, to);
    }
}