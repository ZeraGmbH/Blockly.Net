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
    /// Intercept some exception during execution.
    /// </summary>
    /// <param name="block">Block of interest.</param>
    /// <param name="context">Script execution context.</param>
    /// <param name="original">Original exception seen.</param>
    /// <returns>Exception to propagate.</returns>
    Task<Exception?> InterceptExceptionAsync(Block block, Context context, Exception original);

    /// <summary>
    /// Report that the script as finished execution.
    /// </summary>
    /// <param name="exception">Exception observed</param>
    void ScriptFinished(Exception? exception);

    /// <summary>
    /// Set if debugger is enabled.
    /// </summary>
    bool Enabled { get; set; }

    /// <summary>
    /// Access to the breakpoint management.
    /// </summary>
    IScriptBreakpoints Breakpoints { get; }

    /// <summary>
    /// Current position in script - null while executing.
    /// </summary>
    IScriptPosition? CurrentPosition { get; }

    /// <summary>
    /// Stop at the indicated block.
    /// </summary>
    /// <param name="scriptId">Script to use.</param>
    /// <param name="blockId">Block to stop at.</param>
    void RunTo(string scriptId, string blockId);

    /// <summary>
    /// Prepare to run the script.
    /// </summary>
    /// <param name="mode">How to continue;</param>
    /// <param name="stoppedAt">For synchrounous operations the stop context.</param>
    void Continue(ScriptDebugContinueModes mode, ScriptDebugContext? stoppedAt = null);

    /// <summary>
    /// Information on the current position in script - 
    /// available only when stoppen.
    /// </summary>
    ScriptDebugContext? StoppedAt { get; }
}
