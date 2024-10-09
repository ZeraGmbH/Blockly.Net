using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// 
/// </summary>
public class GroupInfo
{
    /// <summary>
    /// 
    /// </summary>
    [NotNull, Required]
    public string Id { get; set; } = null!;

    /// <summary>
    /// 
    /// </summary>
    [NotNull, Required]
    public string? Name { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [NotNull, Required]
    public List<GroupInfo> Children { get; set; } = [];
}