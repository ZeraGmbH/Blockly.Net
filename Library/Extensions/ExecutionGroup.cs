using System.Text.Json;
using BlocklyNet.Core.Blocks.Variables;
using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;
using BlocklyNet.Scripting.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace BlocklyNet.Extensions;

/// <summary>
/// Execute a function as a Blockly.NET group.
/// </summary>
[CustomBlock(
    "execute_group",
    "ExecutionGroups",
    @"{
        ""message0"": ""ExecutionGroup %1 %2 %3 %4 %5 %6 %7 %8 %9 %10 %11 %12 %13"",
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
                ""type"": ""field_label_serializable"",
                ""name"": ""DETAILS"",
                ""text"": ""Details""
            },
            {
                ""type"": ""input_value"",
                ""name"": ""DETAILS"",
                ""check"": ""String""
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
            },
            {
                ""type"": ""field_label_serializable"",
                ""name"": ""STATUSVAR"",
                ""text"": ""Save Status in""
            },
            {
                ""type"": ""field_variable"",
                ""name"": ""STATUSVAR"",
                ""variable"": ""groupStatus""
            },
            {
                ""type"": ""input_dummy""
            },
            {
                ""type"": ""field_label_serializable"",
                ""name"": ""RESULTVAR"",
                ""text"": ""Save Result in""
            },
            {
                ""type"": ""field_variable"",
                ""name"": ""RESULTVAR"",
                ""variable"": ""groupResult""
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
        var groupResult = context.Engine.BeginGroup(Id, Fields["NAME"], await Values.EvaluateAsync<string?>("DETAILS", context, false));

        if (groupResult == null)
        {
            /* Execute the statement if available - may calculate data needed to create a group result. */
            var statement = Statements.TryGet("DO");

            if (statement != null) await statement.EvaluateAsync(context);

            /* Execute the function to get the group result. */
            var result = await Values.EvaluateAsync<GroupResult>("RESULT", context);

            /* Finish the group. */
            groupResult = context.Engine.EndGroup(result);
        }

        /* Set status. */
        var statusVar = Fields.TryGet("STATUSVAR")?.Value;

        if (!string.IsNullOrEmpty(statusVar)) VariablesSet.Set(context, statusVar, groupResult);

        /* Set result. */
        var resultVar = Fields.TryGet("RESULTVAR")?.Value;

        if (!string.IsNullOrEmpty(resultVar))
        {
            /* Decode the result information - object? will become a JSON document. */
            var rawResult = groupResult.GetResult()?.Result;

            if (rawResult is JsonDocument doc && context.VariableTypes.TryGetValue(resultVar, out var resultType) && !string.IsNullOrEmpty(resultType))
            {
                /* Convert if this is a known type. */
                var modelInfos = context.ServiceProvider.GetRequiredService<IScriptModels>();

                if (modelInfos.Models.TryGetValue(resultType, out var typeInfo) || modelInfos.Enums.TryGetValue(resultType, out typeInfo))
                    rawResult = JsonSerializer.Deserialize(doc, typeInfo.Type, JsonUtils.JsonSettings);
            }

            /* Write to indicated variable. */
            VariablesSet.Set(context, resultVar, rawResult);
        }

        /* Continue with execution. */
        return await base.EvaluateAsync(context);
    }
}
