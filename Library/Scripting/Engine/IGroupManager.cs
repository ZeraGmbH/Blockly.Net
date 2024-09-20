using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// Result of a finished execution group.
/// </summary>
public class GroupStatus
{
    /// <summary>
    /// Unique identifier of the group.
    /// </summary>
    [NotNull, Required]
    public required string Key { get; set; }

    /// <summary>
    /// Optional name of the group.
    /// </summary>
    public required string? Name { get; set; }

    /// <summary>
    /// Result of the execution.
    /// </summary>
    public object? Result { get; set; }

    /// <summary>
    /// All executions started before this group has been finished.
    /// </summary>
    public List<GroupStatus> Children { get; set; } = [];
}

/// <summary>
/// Interface to manage groups.
/// </summary>
public interface IGroupManager
{
    /// <summary>
    /// Reset internal state.
    /// </summary>
    void Clear();

    /// <summary>
    /// Start a new group of execution.
    /// </summary>
    /// <param name="id">Unique identifer of the group.</param>
    /// <param name="name">Optional name of the group.</param>
    void Start(string id, string? name);

    /// <summary>
    /// Finish a group of execution.
    /// </summary>
    /// <param name="result">Result of the execution group.</param>
    void Finish(object? result);

    /// <summary>
    /// Create a nested group manager.
    /// </summary>
    IGroupManager CreateNested();

    /// <summary>
    /// Convert to protocol structure.
    /// </summary>
    /// <returns>List of all group execution results.</returns>
    GroupStatus[] Serialize();
}
