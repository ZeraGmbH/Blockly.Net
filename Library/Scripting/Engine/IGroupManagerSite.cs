namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// Customization interface for group manager.
/// /// </summary>
internal interface IGroupManagerSite
{
    /// <summary>
    /// Starts executing a new group.
    /// </summary>
    /// <param name="status">Status of the new group.</param>
    /// <param name="recover">Set if this is a recovery operation.</param>
    Task BeginExecuteGroupAsync(GroupStatus status, bool recover);

    /// <summary>
    /// Report the end of a group.
    /// </summary>
    /// <param name="status">Status of the finished group.</param>
    Task DoneExecuteGroupAsync(GroupStatus status);
}

