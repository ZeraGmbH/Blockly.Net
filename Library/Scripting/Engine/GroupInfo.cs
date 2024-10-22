using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// Information on an execution group - and all of its children.
/// </summary>
public class GroupInfo
{
    /// <summary>
    /// The blockly idententifier of the group block.
    /// </summary>
    [NotNull, Required]
    public string Id { get; set; } = null!;

    /// <summary>
    /// The user defined name of the group.
    /// </summary>
    [NotNull, Required]
    public string? Name { get; set; }

    /// <summary>
    /// All nested groups.
    /// </summary>
    [NotNull, Required]
    public List<GroupInfo> Children { get; set; } = [];

    /// <summary>
    /// All scripts (potentially) called by this block.
    /// </summary>
    [NotNull, Required]
    public List<string> Scripts { get; set; } = [];
}