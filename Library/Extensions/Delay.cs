using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;

namespace BlocklyNet.Extensions;

/// <summary>
/// Set a progress for the current script execution.
/// </summary>
[CustomBlock(
    "delay",
    "",
    @"{
        ""message0"": ""Delay %1 %2 %3"",
        ""args0"": [
            {
                ""type"": ""input_dummy""
            },
            {
                ""type"": ""field_label_serializable"",
                ""name"": ""DELAY"",
                ""text"": ""Delay (ms)""
            },
            {
                ""type"": ""input_value"",
                ""name"": ""DELAY"",
                ""check"": ""Number""
            }
        ],
        ""previousStatement"": null,
        ""nextStatement"": null,
        ""colour"": ""#107159"",
        ""tooltip"": ""Delay execution"",
        ""helpUrl"": """"
    }",
    @"{
        ""inputs"": {
            ""DELAY"": {
                ""shadow"": {
                    ""type"": ""math_number"",
                    ""fields"": {
                        ""NUM"": ""0""
                    }
                }
            }
        }
    }"
)]
public class Delay : Block
{
    /// <inheritdoc/>
    public override async Task<object?> Evaluate(Context context)
    {
        var delay = (int)await Values.Evaluate<double>("DELAY", context);

        if (delay > 0) await Task.Delay(delay, context.Cancellation);

        return await base.Evaluate(context);
    }
}
