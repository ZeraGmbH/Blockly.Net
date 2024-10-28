using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// Former result of a group execution and an option for retry.
/// </summary>
public class GroupRepeat : GroupStatus<GroupRepeat>
{
    /// <summary>
    /// How to execute the group.
    /// </summary>
    [NotNull, Required]
    public GroupRepeatType Repeat { get; set; }

    /// <summary>
    /// Convert to a regulat status.
    /// </summary>
    /// <returns>Regular status.</returns>
    public GroupStatus ToStatus()
        => JsonSerializer.Deserialize<GroupStatus>(JsonSerializer.Serialize(this, JsonUtils.JsonSettings), JsonUtils.JsonSettings)!;
}