using BlocklyNet.Core.Model;
using BlocklyNet.Scripting.Debugger;
using BlocklyNet.Scripting.Logging;
using Microsoft.Extensions.Logging;

namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// 
/// </summary>
public interface IScriptSite
{
    /// <summary>
    /// The controlling engine.
    /// </summary>
    IScriptEngine Engine { get; }

    /// <summary>
    /// Report the current logging helper of the script engine.
    /// </summary>
    ILogger Logger { get; }

    /// <summary>
    /// Dependency injection manager.
    /// </summary>
    IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Report if the current script should cancel as soon as possible.
    /// </summary>
    CancellationToken Cancellation { get; }

    /// <summary>
    /// Set progress information.
    /// </summary>
    /// <param name="info">Script dependant progress data.</param>
    /// <param name="progress">Progress as a number between 0 and 1.</param>
    /// <param name="name">Optional display name of the progress.</param>
    /// <param name="addEstimation">If set add time to end estimtation if possible.</param>
    /// <param name="noVisualisation">If set frontend should not display this progress.</param>
    void ReportProgress(object info, double? progress, string? name, bool? addEstimation, bool? noVisualisation);

    /// <summary>
    ///  Execute Blockly XML Script and report variables.
    /// </summary>
    /// <param name="scriptAsXml">Blockly XML representation of a workspace.</param>
    /// <param name="presets">Preset variables.</param>
    /// <returns>Variables after execution.</returns>
    Task<object?> EvaluateAsync(string scriptAsXml, Dictionary<string, object?> presets);

    /// <summary>
    /// Start a nested script.
    /// </summary>
    /// <param name="request">Script to start.</param>
    /// <param name="options">Detailed configuration.</param>
    /// <returns>Result of the script.</returns>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    Task<TResult> RunAsync<TResult>(StartScript request, StartScriptOptions? options = null);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="type"></param>
    /// <param name="delay"></param>
    /// <param name="required"></param>
    /// <typeparam name="T">Expected type of the response.</typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    Task<T?> GetUserInputAsync<T>(string key, string? type = null, double? delay = null, bool? required = false);

    /// <summary>
    /// Call just before a block is executed.
    /// </summary>
    /// <param name="block">The block to execute.</param>
    /// <param name="context">Script execution environment.</param>
    /// <param name="reason">Reason why the debugger is informed.</param>
    Task SingleStepAsync(Block block, Context context, ScriptDebuggerStopReason reason);

    /// <summary>
    /// Start the execution of a new group.
    /// </summary>
    /// <param name="key">Unique identifier of the group.</param>
    /// <param name="name">Optional name of the group.</param>
    /// <param name="details">Optional details of the group executions.</param>
    /// <returns>null if the execution started, set if the result from
    /// a previous execution has been reuses.</returns>
    Task<GroupStatus?> BeginGroupAsync(string key, string? name, string? details);

    /// <summary>
    /// End the execution of the current group.
    /// </summary>
    Task<GroupStatus> EndGroupAsync(GroupResult result);

    /// <summary>
    /// Report the status of a single group execution.
    /// </summary>
    /// <param name="index">Zero-based index of the group.</param>
    /// <returns>Information on the group.</returns>
    GroupStatus GetGroupStatus(int index);

    /// <summary>
    /// Report the currently running script.
    /// </summary>
    IScript? CurrentScript { get; }

    /// <summary>
    /// Report the outer script.
    /// </summary>
    IScript? MainScript { get; }

    /// <summary>
    /// Update the current log entry.
    /// </summary>
    Task UpdateLogAsync();

    /// <summary>
    /// Attach or detach the debugger associated with this script site.
    /// </summary>
    /// <param name="debugger">Debugger to use or null to detach.</param>
    void SetDebugger(IScriptDebugger? debugger);
}

/// <summary>
/// 
/// </summary>
public interface IScriptSite<TLogType> : IScriptSite where TLogType : ScriptLoggingResult
{
    /// <summary>
    /// Report the currently running script.
    /// </summary>
    new IScript<TLogType>? CurrentScript { get; }

    /// <summary>
    /// Report the outer script.
    /// </summary>
    new IScript<TLogType>? MainScript { get; }
}