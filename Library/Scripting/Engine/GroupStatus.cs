using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// Result of a finished execution group.
/// </summary>
public class GroupStatusCommon
{
    /// <summary>
    /// Set to indicate that this is no a real group but a separate script.
    /// </summary>
    [NotNull, Required]
    public bool IsScript { get; set; }

    /// <summary>
    /// Unique identifier of the group.
    /// </summary>
    [NotNull, Required]
    public string Key { get; set; } = null!;

    /// <summary>
    /// Optional name of the group.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Result of the execution.
    /// </summary>
    public string? Result { get; set; }

    /// <summary>
    /// Save result of a group in a serialized form.
    /// </summary>
    /// <param name="result">Result to remember.</param>
    /// <remarks>Without any external manipulation of JsonUtils.JsonSettings all
    /// type informations will be lost.</remarks>
    public void SetResult(GroupResult result) => Result = JsonSerializer.Serialize(result, JsonUtils.JsonSettings);

    /// <summary>
    /// Reconstruct the result from the JSON string.
    /// </summary>
    /// <returns>Recostructed result.</returns>
    public GroupResult? GetResult() => JsonSerializer.Deserialize<GroupResult>(Result ?? "null", JsonUtils.JsonSettings);
}

/// <summary>
/// Result of a finished execution group.
/// </summary>
public class GroupStatus<TChild> : GroupStatusCommon where TChild : GroupStatus<TChild>
{
    /// <summary>
    /// All executions started before this group has been finished.
    /// </summary>
    [NotNull, Required]
    public List<TChild> Children { get; set; } = [];
}

/// <summary>
/// Status information.
/// </summary>
public class GroupStatus : GroupStatus<GroupStatus> { }