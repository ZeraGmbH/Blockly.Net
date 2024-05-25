using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;

namespace BlocklyNet.Extensions;

/// <summary>
/// Set a progress for the current script execution.
/// </summary>
[CustomBlock(
    "try_catch_finally",
    "",
    @"{
        ""message0"": ""Try / Catch / Finally %1 Try %2 Catch %3 Finally %4"",
        ""args0"": [
            {
                ""type"": ""input_dummy""
            },
            {
                ""type"": ""input_statement"",
                ""name"": ""TRY""
            },
            {
                ""type"": ""input_statement"",
                ""name"": ""CATCH""
            },
            {
                ""type"": ""input_statement"",
                ""name"": ""FINALLY""
            }
        ],
        ""previousStatement"": null,
        ""nextStatement"": null,
        ""colour"": ""#107159"",
        ""tooltip"": ""Execute block with exception handling"",
        ""helpUrl"": """"
    }",
    ""
)]
public class TryCatchFinally : Block
{
    /// <inheritdoc/>
    public override async Task<object?> Evaluate(Context context)
    {
        try
        {
            /* Fetch the block to run. */
            var exec = Statements.TryGet("TRY");

            if (exec != null) await exec.Evaluate(context);
        }
        catch (Exception)
        {
            /* Fetch the block to run. */
            var report = Statements.TryGet("CATCH");

            if (report != null) await report.Evaluate(context);
        }
        finally
        {
            /* Fetch the block to run. */
            var cleanup = Statements.TryGet("FINALLY");

            if (cleanup != null) await cleanup.Evaluate(context);
        }

        /* Proceed with next block in chain. */
        return await base.Evaluate(context);
    }
}
