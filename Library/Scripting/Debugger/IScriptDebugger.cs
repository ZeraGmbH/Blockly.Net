using BlocklyNet.Core.Model;

namespace BlocklyNet.Scripting.Debugger;

/// <summary>
/// Script debugger interface.
/// </summary>
public interface IScriptDebugger
{
    /// <summary>
    /// Inform the debugger on the status of the execution.
    /// </summary>
    /// <param name="block">Block of interest.</param>
    /// <param name="context">Script execution context.</param>
    /// <param name="reason">Reason for the information.</param>
    Task InterceptAsync(Block block, Context context, ScriptDebuggerStopReason reason);

    /// <summary>
    /// Report that the script as finished execution.
    /// </summary>
    /// <param name="exception">Exception observed</param>
    void ScriptFinished(Exception? exception);
}