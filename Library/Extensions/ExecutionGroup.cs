using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;

namespace BlocklyNet.Extensions;

/// <summary>
/// Execute a function as a Blockly.NET group.
/// </summary>
[CustomBlock(
    "execute_group",
    "",
    @"{
        ""message0"": ""ExecutionGroup %1 %2 %3"",
        ""args0"": [
            {
                ""type"": ""input_dummy""
            },
            {
                ""type"": ""field_label_serializable"",
                ""name"": ""RESULT"",
                ""text"": ""Result""
            },
            {
                ""type"": ""input_value"",
                ""name"": ""RESULT""
            }
        ],
        ""previousStatement"": null,
        ""nextStatement"": null,
        ""colour"": ""#107159"",
        ""tooltip"": ""Execute something as a group."",
        ""helpUrl"": """"
    }",
    ""
)]
public class ExecutionGroup : Block
{
    /// <inheritdoc/>
    public override async Task<object?> EvaluateAsync(Context context)
    {
        /* Register the group. */
        context.Engine.BeginGroup(Id);

        /* Execute the function to get the group result. */
        var groupResult = await Values.EvaluateAsync("RESULT", context);

        /* Finish the group. */
        context.Engine.EndGroup(groupResult);

        /* Continue with execution. */
        return await base.EvaluateAsync(context);
    }
}
