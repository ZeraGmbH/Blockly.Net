using Microsoft.Extensions.Logging;

namespace BlocklyNet.Scripting.Engine;

partial class ScriptEngine<TLogType>
{
    /// <summary>
    /// Child progress information - nested by depth.
    /// </summary>
    private readonly List<List<ScriptSite>> _allProgress = [];

    /// <summary>
    /// Last progress reported.
    /// </summary>
    private ProgressDetails? _lastProgressValue;

    /// <inheritdoc/>
    public void ReportProgress(object info, double? progress, string? name)
    {
        _lastProgressValue = new() { Progress = progress ?? 0, Name = name, Info = info };

        ReportProgress(info, 0);
    }

    private void ReportProgress(object info, int depth)
    {
        using (Lock.Wait())
        {
            if (_active == null) return;

            /* Check for nested progress. */
            var nextProgress = new ScriptProgress { Info = info, JobId = _active.JobId };

            if (depth > 0)
            {
                /* Outer script must have sent any progress. */
                if (_lastProgress == null) return;

                /* Use last progress from outer. */
                nextProgress.Info = _lastProgress.Info;
            }

            /* Fill in all nested progress in order. */
            if (_lastProgressValue != null) nextProgress.AllProgress.Add(_lastProgressValue);

            foreach (var list in _allProgress)
                foreach (var site in list)
                    if (site.LastProgress != null)
                        nextProgress.AllProgress.Add(site.LastProgress);

            /* Remember for reconnect. */
            _lastProgress = nextProgress;
            _lastProgress.GroupStatus = SerializeGroupStatus();

            _context?
                .SendAsync(ScriptEngineNotifyMethods.Progress, _lastProgress)
                .ContinueWith(
                    t => Logger.LogError("Failed to forward progress: {Exception}", t.Exception?.Message),
                    CancellationToken.None,
                    TaskContinuationOptions.NotOnRanToCompletion,
                    TaskScheduler.Current)
                .Touch();
        }
    }
}
