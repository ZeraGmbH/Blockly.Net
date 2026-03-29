using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlocklyNet.Scripting.Debugger;

/// <summary>
/// Position inside a script.
/// </summary>
public interface IScriptPosition
{
    /// <summary>
    /// Primary key of the corresponding script definition.
    /// </summary>
    [NotNull, Required]
    string ScriptId { get; }

    /// <summary>
    /// Unique id of the block.
    /// </summary>
    [NotNull, Required]
    string BlockId { get; }
}
