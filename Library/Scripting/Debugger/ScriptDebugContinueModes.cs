using System.Text.Json.Serialization;

namespace BlocklyNet.Scripting.Debugger;

/// <summary>
/// How to continue after a break.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ScriptDebugContinueModes
{
    /// <summary>
    /// Respect settings for breakpoints and exception handling.
    /// </summary>
    Normal = 0,

    /// <summary>
    /// Stop again after the current block has been
    /// evaluated completly.
    /// </summary>
    StepOver = 1,

    /// <summary>
    /// Stop after the current block.
    /// </summary>
    StepInto = 2,

    /// <summary>
    /// Stop when the current context is left.
    /// </summary>
    StepOut = 3,

    /// <summary>
    /// Leave the current script.
    /// </summary>
    LeaveScript = 4,
}