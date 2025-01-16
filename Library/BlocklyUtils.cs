namespace BlocklyNet;

/// <summary>
/// Some implementation helpers.
/// </summary>
public static class BlockyUtils
{
    /// <summary>
    /// Simulate task access to avoid warnings.
    /// </summary>
    /// <param name="task">Some task.</param>
    public static void Touch(this Task task) { }
}