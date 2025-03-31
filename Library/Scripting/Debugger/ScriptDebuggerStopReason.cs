namespace BlocklyNet.Scripting.Debugger;

/// <summary>
/// Reasons for the debugger to get informed.
/// </summary>
public enum ScriptDebuggerStopReason
{
    /// <summary>
    /// Starting a block.
    /// </summary>
    Enter,

    /// <summary>
    /// Block itself finished, next block will be started.
    /// </summary>
    Leave,

    /// <summary>
    /// /// Block tree has finished.
    /// </summary>
    Finish,
}
