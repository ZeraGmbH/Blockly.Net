using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlocklyNet.Scripting.Debugger;

/// <summary>
/// Information on a script breakpoint.
/// </summary>
public interface IScriptLocation
{   /// <summary>
    /// Unique identifier of the script.
    /// </summary>
    [NotNull, Required]
    string ScriptId { get; }

    /// <summary>
    /// Unique identitifier of the block.
    /// </summary>
    [NotNull, Required]
    string BlockId { get; }
}
