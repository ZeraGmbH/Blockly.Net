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
    protected override async Task<object?> EvaluateAsync(Context context)
    {
        try
        {
            /* Fetch the block to run. */
            var exec = Statements.TryGet("TRY");

            if (exec != null) await exec.EvaluateAsync(context);
        }
        catch (ReturnProcedureIfException)
        {
            throw;
        }
        catch (Exception e)
        {
            /* Always remember. */
            var lastException = context.LastException;

            context.LastException = e;

            try
            {
                /* Fetch the block to run. */
                var report = Statements.TryGet("CATCH");

                if (report != null) await report.EvaluateAsync(context);
            }
            finally
            {
                /* Reset. */
                context.LastException = lastException;
            }
        }
        finally
        {
            /* Fetch the block to run. */
            var cleanup = Statements.TryGet("FINALLY");

            if (cleanup != null) await cleanup.EvaluateAsync(context);
        }

        /* Proceed with next block in chain. */
        return await base.EvaluateAsync(context);
    }
}
