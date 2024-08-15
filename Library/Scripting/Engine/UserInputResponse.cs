using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// Reports the requested input - currently only numbers
/// are supported.
/// </summary>
public class UserInputResponse : UserInputRequestBase
{
    /// <summary>
    /// The input provided by the user.
    /// </summary>
    [Required, NotNull]
    public object? Value { get; set; }
}
