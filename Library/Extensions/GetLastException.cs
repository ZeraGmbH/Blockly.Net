using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;

namespace BlocklyNet.Extensions;

/// <summary>
/// Get the latest exception.
/// </summary>
[CustomBlock(
    "get_last_exception",
    "",
    @"{
        ""message0"": ""GetLastException"",
        ""args0"": [],
        ""output"": ""String"",
        ""colour"": ""#107159"",
        ""tooltip"": ""Get Exception of current Catch."",
        ""helpUrl"": """"
    }",
    ""
)]
public class GetLastException : Block
{
    /// <inheritdoc/>
    public override Task<object?> EvaluateAsync(Context context) => Task.FromResult<object?>(context.LastException?.Message);
}
