
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlocklyNet.Scripting.Generic;

/// <summary>
/// Result of a script execution.
/// </summary>
public class GenericResult
{
    /// <summary>
    /// Variable named result from the script.
    /// </summary>
    [Required, NotNull]
    public object? Result { get; set; }

    /// <summary>
    /// Type of the result to allow client to display it correctly.
    /// </summary>
    public string? ResultType { get; set; }

    /// <summary>
    /// The script currently executing.
    /// </summary>
    [Required, NotNull]
    public string ScriptId { get; set; } = null!;
}