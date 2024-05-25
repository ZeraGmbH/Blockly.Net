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
}
