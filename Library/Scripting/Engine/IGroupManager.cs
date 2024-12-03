namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// Interface to manage groups.
/// </summary>
public interface ISiteGroupManager
{
    /// <summary>
    /// Start a new group of execution.
    /// </summary>
    /// <param name="id">Unique identifer of the group.</param>
    /// <param name="name">Optional name of the group.</param>
    /// <param name="details">Optional detail information for the execution.</param>
    /// <returns>Set if the execution started, unset if the result from
    /// a previous execution has been reuses.</returns>
    bool Start(string id, string? name, string? details);

    /// <summary>
    /// Finish a group of execution.
    /// </summary>
    /// <param name="result">Result of the execution group.</param>
    void Finish(GroupResult result);

    /// <summary>
    /// Create a nested group manager.
    /// </summary>
    /// <param name="scriptId">Unique identifier of the script.</param>
    /// <param name="name">Name of the Script.</param>
    IGroupManager CreateNested(string scriptId, string name);

    /// <summary>
    /// Get the groups status for some group.
    /// </summary>
    /// <param name="index">Zero-base index of the group.</param>
    /// <returns>Information on the group execution.</returns>
    GroupStatus this[int index] { get; }
}

/// <summary>
/// Interface to manage groups.
/// </summary>
public interface IGroupManager : ISiteGroupManager
{
    /// <summary>
    /// Reset internal state.
    /// </summary>
    void Reset(IEnumerable<GroupRepeat>? previous);

    /// <summary>
    /// Convert to protocol structure.
    /// </summary>
    /// <param name="includeRepeat">Include repetition information for all groups not executed.</param>
    /// <returns>List of all group execution results.</returns>
    List<GroupStatus> Serialize(bool includeRepeat = false);

    /// <summary>
    /// Generate a result from the groups.
    /// </summary>
    /// <returns>List of results if any are present.</returns>
    List<object?>? CreateFlatResults();
}