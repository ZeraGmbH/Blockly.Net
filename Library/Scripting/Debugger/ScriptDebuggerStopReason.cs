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
    /// Block itself finished, next block will be started.
    /// </summary>
    Leave = 1,

    /// <summary>
    /// /// Block tree has finished.
    /// </summary>
    Finish = 2,
}
