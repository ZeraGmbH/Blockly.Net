namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// Progress helper.
/// </summary>
public class ProgressManager
{
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
    /// Add a new progress.
    /// </summary>
    /// <param name="info">Progress data.</param>
    /// <param name="progress">Progress value between 0 and 1.</param>
    /// <param name="name">Optional name of the progress.</param>
    /// <param name="addEstimation">Set to add time to finish estimation - if possible.</param>
    public void Update(object info, double? progress, string? name, bool? addEstimation)
    {
        Latest = new() { Progress = progress ?? 0, Name = name, Info = info };
    }
}
