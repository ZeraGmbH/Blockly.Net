namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// Details of a progress information.
/// </summary>
public class ProgressDetails
{
    /// <summary>
    /// Optional name of the progress.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Progress value between 0 and 1 - both inclusive.
    /// </summary>
    public double Progress { get; set; }

    /// <summary>
    /// The optional progress information depending on the type of script.
    /// </summary>
    public object? Info { get; set; }

    /// <summary>
    /// If possible an estimation on long it takes to
    /// get to a progress of 1 - in seconds.
    /// </summary>
    public double? EstimatedRemainingSeconds { get; set; }

    /// <summary>
    /// Set to hide this progress from the frontend.
    /// </summary>
    public bool? NoVisualisation { get; set; }
}
