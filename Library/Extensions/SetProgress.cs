using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;
using BlocklyNet.Scripting.Generic;

namespace BlocklyNet.Extensions;

/// <summary>
/// Set a progress for the current script execution.
/// </summary>
[CustomBlock(
    "set_progress",
    "",
    @"{
        ""message0"": ""SetProgress %1 Name of the progress %2 Progress (%%) %3 Extra data %4 Type of extra data %5 Add Time Estimation %6 Do not visualize %7"",
        ""args0"": [
            {
                ""type"": ""input_dummy""
            },
            {
                ""type"": ""input_value"",
                ""name"": ""NAME"",
                ""check"": ""String""
            },
            {
                ""type"": ""input_value"",
                ""name"": ""PROGRESS"",
                ""check"": ""Number""
            },
            {
                ""type"": ""input_value"",
                ""name"": ""PAYLOAD""
            },
            {
                ""type"": ""input_value"",
                ""name"": ""PAYLOADTYPE"",
                ""check"": ""String""
            },
            {
                ""type"": ""input_value"",
                ""name"": ""ADDESTIMATION"",
                ""check"": ""Boolean""
            },
            {
                ""type"": ""input_value"",
                ""name"": ""NOVISUALIZATION"",
                ""check"": ""Boolean""
            }
        ],
        ""previousStatement"": null,
        ""nextStatement"": null,
        ""colour"": ""#107159"",
        ""tooltip"": ""Set script execution progress in percentage"",
        ""helpUrl"": """"
    }",
    @"{
        ""inputs"": {
            ""NAME"": {
                ""shadow"": {
                    ""type"": ""text"",
                    ""fields"": {
                        ""TEXT"": """"
                    }
                }
            },
            ""PROGRESS"": {
                ""shadow"": {
                    ""type"": ""math_number"",
                    ""fields"": {
                        ""NUM"": ""0""
                    }
                }
            },
            ""PAYLOADTYPE"": {
                ""shadow"": {
                    ""type"": ""text"",
                    ""fields"": {
                        ""TEXT"": """"
                    }
                }
            },
            ""ADDESTIMATION"": {
                ""shadow"": {
                ""type"": ""logic_boolean"",
                ""fields"": {
                    ""BOOL"": ""FALSE""
                }
                }
            },
            ""NOVISUALIZATION"": {
                ""shadow"": {
                ""type"": ""logic_boolean"",
                ""fields"": {
                    ""BOOL"": ""FALSE""
                }
                }
            }
        }
    }"
)]
public class SetProgress : Block
{
    /// <inheritdoc/>
    protected override async Task<object?> EvaluateAsync(Context context)
    {
        var script = context.Engine.MainScript as IGenericScript;
        var progress = await Values.EvaluateAsync<double>("PROGRESS", context);
        var name = await Values.EvaluateAsync<string?>("NAME", context, false);

        context.Engine.ReportProgress(
            new GenericProgress
            {
                Payload = await Values.EvaluateAsync("PAYLOAD", context, false),
                PayloadType = await Values.EvaluateAsync<string>("PAYLOADTYPE", context, false),
                Percentage = progress,
                ScriptId = script?.Request.ScriptId,
            },
            progress / 100d,
            name,
            await Values.EvaluateAsync<bool?>("ADDESTIMATION", context, false) == true,
            await Values.EvaluateAsync<bool?>("NOVISUALIZATION", context, false) == true
        );

        return await base.EvaluateAsync(context);
    }
}
