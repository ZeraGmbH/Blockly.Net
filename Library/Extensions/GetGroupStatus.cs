using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;

namespace BlocklyNet.Extensions;

/// <summary>
/// Retrieve the status of a group execution.
/// </summary>
[CustomBlock(
    "get_group_execution_status",
    "",
    @"{
        ""message0"": ""GetGroupStatus %1 %2 %3"",
        ""args0"": [
            {
                ""type"": ""input_dummy""
            },
            {
                ""type"": ""field_label_serializable"",
                ""name"": ""INDEX"",
                ""text"": ""Index""
            },
            {
                ""type"": ""input_value"",
                ""name"": ""INDEX"",
                ""check"": ""Number""
            }
        ],
        ""output"": ""group_execution_status"",
        ""colour"": ""#107159"",
        ""tooltip"": ""Get the Status of a Group Execution."",
        ""helpUrl"": """"
    }",
    @"{
        ""inputs"": {
            ""INDEX"": {
                ""shadow"": {
                    ""type"": ""math_number"",
                    ""fields"": {
                        ""NUM"": ""1""
                    }
                }
            }
        }
    }"
)]
public class GetGroupStatus : Block
{
    /// <inheritdoc/>
    public override async Task<object?> EvaluateAsync(Context context)
    {
        var index = (int)await Values.EvaluateAsync<double>("INDEX", context);

        return context.Engine.GetGroupStatus(index - 1);
    }
}
