using System.Text.Json.Serialization;

namespace BlocklyNet.Scripting.Debugger;

/// <summary>
/// Reasons for the debugger to get informed.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ScriptDebuggerStopReason
{
    /// <summary>
    /// Starting a block.
    /// </summary>
    Enter = 0,

    /// <summary>
    /// Block tree has finished.
    /// </summary>
    Leave = 1,

    /// <summary>
    /// Script has finished.
    /// </summary>
    ScriptDone = 2,

    /// <summary>
    /// Detected an exception.
    /// </summary>
    Exception = 3,
}
