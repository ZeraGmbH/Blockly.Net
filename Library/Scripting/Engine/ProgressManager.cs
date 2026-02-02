namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// Progress helper.
/// </summary>
public class ProgressManager
{
    /// <summary>
    /// Get current time - may be overwritten in test.
    /// </summary>
    internal Func<DateTime> Now = () => DateTime.UtcNow;

    /// <summary>
    /// Maximum time to finish estimation value.
    /// </summary>
    private static readonly TimeSpan MaximumRemainingSeconds = TimeSpan.FromDays(366);

    /// <summary>
    /// Latest progress seen.
    /// </summary>
    public ProgressDetails? Latest { get; private set; }

    /// <summary>
    /// Reset progress data.
    /// </summary>
    public void Reset()
    {
        Latest = null;
    }

    /// <summary>
    /// Time of the progress with the smallest value.
    /// </summary>
    private DateTime _startTime;

    /// <summary>
    /// Progress with the smallest value.
    /// </summary>
    private double? _startProgress;

    /// <summary>
    /// Add a new progress.
    /// </summary>
    /// <param name="info">Progress data.</param>
    /// <param name="progress">Progress value between 0 and 1.</param>
    /// <param name="name">Optional name of the progress.</param>
    /// <param name="addEstimation">Set to add time to finish estimation - if possible.</param>
    /// <param name="noVisualisation">Set to hide progress from beeing shown.</param>
    public void Update(object info, double? progress, string? name, bool? addEstimation, bool? noVisualisation)
    {
        // This is the very first.
        if (Latest == null) _startProgress = null;

        // Update as requested.
        Latest = new() { Progress = progress ?? 0, Name = name, Info = info, NoVisualisation = noVisualisation };

        // Can not estimate at all.
        if (Latest.Progress < 0 || Latest.Progress > 1) return;

        // Our very first progress - or a reset on backrunning progress.
        var now = Now();

        if (_startProgress == null || Latest.Progress < _startProgress)
        {
            // Remember first valid or minimum progress as a reference.
            _startTime = now;
            _startProgress = Latest.Progress;

            return;
        }

        // No estimation requested.
        if (addEstimation != true) return;

        // Check for time expired so far - and respect some lower threshold.
        var deltaTime = now - _startTime;
        var deltaProgress = Latest.Progress - _startProgress;

        if (deltaTime.TotalSeconds < 1 || deltaProgress < 0.01) return;

        // Time to complete the full progress.
        var totalEstimation = deltaTime / deltaProgress;

        // Remaining time.
        var remainingEstimation = (1 - Latest.Progress) * totalEstimation;

        // Check for regular number - esp. overflow.
        if (remainingEstimation < MaximumRemainingSeconds)
            Latest.EstimatedRemainingSeconds = remainingEstimation.Value.TotalSeconds;
    }
}
