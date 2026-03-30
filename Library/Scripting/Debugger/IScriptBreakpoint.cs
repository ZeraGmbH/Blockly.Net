using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlocklyNet.Scripting.Debugger;

/// <summary>
/// Information on a script breakpoint.
/// </summary>
public interface IScriptBreakpoint : IScriptLocation
{
    /// <summary>
    /// Set if breakpoint is enabled.
    /// </summary>
    [NotNull, Required]
    bool Enabled { get; set; }

    /// <summary>
    /// Optional description of the breakpoint - what 
    /// the user sees.
    /// </summary>
    string? Description { get; }
}
