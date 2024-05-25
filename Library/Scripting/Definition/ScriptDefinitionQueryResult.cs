using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlocklyNet.Scripting.Definition;

/// <summary>
/// 
/// </summary>
public class ScriptDefinitionQueryResult
{
    /// <summary>
    /// Unique identifier of the script.
    /// </summary>
    [Required, NotNull]
    public required string Id { get; set; }

    /// <summary>
    /// Display name of the script.
    /// </summary>
    [Required, NotNull]
    public required string Name { get; set; }
}