using Microsoft.Extensions.Logging;

namespace BlocklyNet.Scripting.Engine;

partial class ScriptEngine<TLogType>
{
    /// <inheritdoc/>
    public Task UpdateLogAsync() => CurrentScript == null ? Task.CompletedTask : UpdateResultLogEntryAsync(CurrentScript, null, false);

    /// <summary>
    /// Update the log entry for a script.
    /// </summary>
    /// <param name="script">Script to update.</param>
    /// <param name="parent">Optional the parent script - null for the root.</param>
    /// <param name="final">Set if the script is now finished - unset for updates during the execution.</param>
    protected async Task UpdateResultLogEntryAsync(IScript<TLogType> script, IScript<TLogType>? parent, bool final)
    {
        using (Lock.Wait())
            try
            {
                /* For the outer script always add the current status of the exeuction groups. */
                if (parent == null) script.SetGroups(SerializeGroupStatus(true));

                /* Mark as finished. */
                if (final) script.ResultForLogging.Finished = DateTime.Now;

                var measurementId = await script.WriteToLogAsync();

                /* Register in parent script. */
                if (parent == null || parent.ResultForLogging.Children.Contains(measurementId)) return;

                /* First update for parent. */
                await parent.RegisterChildAsync(measurementId);
            }
            catch (Exception e)
            {
                Logger.LogError("Unable to create log entry: {Exception}", e.Message);

                return;
            }

        try
        {
            /* Must forward to parent if child list has been updated. */
            await UpdateLogAsync();
        }
        catch (Exception e)
        {
            Logger.LogError("Unable to create log entry: {Exception}", e.Message);
        }
    }
}
