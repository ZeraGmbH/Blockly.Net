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
        ""message0"": ""SetProgress %1 %2 %3 %4 %5 %6 %7 %8 %9"",
        ""args0"": [
            {
                ""type"": ""input_dummy""
            },
            {
                ""type"": ""field_label_serializable"",
                ""name"": ""NAME"",
                ""text"": ""Name of the progress""
            },
            {
                ""type"": ""input_value"",
                ""name"": ""NAME"",
                ""check"": ""String""
            },
            {
                ""type"": ""field_label_serializable"",
                ""name"": ""PROGRESS"",
                ""text"": ""Progress (%)""
            },
            {
                ""type"": ""input_value"",
                ""name"": ""PROGRESS"",
                ""check"": ""Number""
            },
            {
                ""type"": ""field_label_serializable"",
                ""name"": ""PAYLOAD"",
                ""text"": ""Extra data""
            },
            {
                ""type"": ""input_value"",
                ""name"": ""PAYLOAD""
            },
            {
                ""type"": ""field_label_serializable"",
                ""name"": ""PAYLOADTYPE"",
                ""text"": ""Type of extra data""
            },
            {
                ""type"": ""input_value"",
                ""name"": ""PAYLOADTYPE"",
                ""check"": ""String""
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
            }
        }
    }"
)]
public class SetProgress : Block
{
    /// <inheritdoc/>
    public override async Task<object?> Evaluate(Context context)
    {
        var script = context.Engine.MainScript as IGenericScript;
        var progress = await Values.Evaluate<double>("PROGRESS", context);
        var name = await Values.Evaluate<string?>("NAME", context, false);

        context.Engine.ReportProgress(new GenericProgress
        {
            Payload = await Values.Evaluate("PAYLOAD", context, false),
            PayloadType = await Values.Evaluate<string>("PAYLOADTYPE", context, false),
            Percentage = progress,
            ScriptId = script?.Request.ScriptId
        }, progress / 100d, name);

        return await base.Evaluate(context);
    }
}
