using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;
using BlocklyNet.Scripting.Engine;

namespace BlocklyNet.Extensions;

/// <summary>
/// Execute a function as a Blockly.NET group.
/// </summary>
[CustomBlock(
    "execute_group",
    "ExecutionGroups",
    @"{
        ""message0"": ""ExecutionGroup %1 %2 %3 %4 %5 %6"",
        ""args0"": [
            {
                ""type"": ""input_dummy""
            },
            {
                ""type"": ""field_input"",
                ""name"": ""NAME"",
                ""text"": ""Name of the Group""
            },
            {
                ""type"": ""input_dummy"",
                ""name"": ""NAME""
            },
            {
                ""type"": ""input_statement"",
                ""name"": ""DO""
            },
            {
                ""type"": ""field_label_serializable"",
                ""name"": ""RESULT"",
                ""text"": ""Result""
            },
            {
                ""type"": ""input_value"",
                ""name"": ""RESULT"",
                ""check"": ""group_execution_result""
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
        var name = Fields["NAME"];

        if (context.Engine.BeginGroup(Id, name))
        {
            /* Execute the statement if available - may calculate data needed to create a group result. */
            var statement = Statements.TryGet("DO");

            if (statement != null) await statement.EvaluateAsync(context);

            /* Execute the function to get the group result. */
            var groupResult = await Values.EvaluateAsync<GroupResult>("RESULT", context);

            /* Finish the group. */
            context.Engine.EndGroup(groupResult);
        }

        /* Continue with execution. */
        return await base.EvaluateAsync(context);
    }
}
