using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// A script requires some user input - currently only
/// numbers are supported.
/// </summary>
public class UserInputRequestBase
{
    /// <summary>
    /// The unique identifier of the active script.
    /// </summary>
    [Required, NotNull]
    public string JobId { get; set; } = null!;

    /// <summary>
    /// Some unique input key identifying the requested
    /// input. This can be used for dynamic translations.
    /// </summary>
    [Required, NotNull]
    public string Key { get; set; } = null!;

    /// <summary>
    /// Optional type of value. May be set to void to just
    /// report something without requesting a value.
    /// </summary>
    public string? ValueType { get; set; }
}

/// <summary>
/// A script requires some user input - currently only
/// numbers are supported.
/// </summary>
public class UserInputRequest : UserInputRequestBase
{
    /// <summary>
    /// Exact time the request was started.
    /// </summary>
    [Required, NotNull]
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// Optional delay to auto-close the request.
    /// </summary>
    public double? SecondsToAutoClose { get; set; }
}
