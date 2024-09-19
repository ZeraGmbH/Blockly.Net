using BlocklyNet.Core.Model;
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
    /// Report the currently running script.
    /// </summary>
    IScript? CurrentScript { get; }

    /// <summary>
    /// Report the outer script.
    /// </summary>
    IScript? MainScript { get; }

    /// <summary>
    /// Set progress information.
    /// </summary>
    /// <param name="info">Script dependant progress data.</param>
    /// <param name="progress">Progress as a number between 0 and 1.</param>
    /// <param name="name">Optional display name of the progress.</param>
    void ReportProgress(object info, double? progress, string? name);

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
    /// <typeparam name="T">Expected type of the response.</typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    Task<T?> GetUserInputAsync<T>(string key, string? type = null, double? delay = null);

    /// <summary>
    /// Call just before a block is executed.
    /// </summary>
    /// <param name="block">The block to execute.</param>
    Task SingleStepAsync(Block block);

    /// <summary>
    /// Start the execution of a new group.
    /// </summary>
    /// <param name="key">Unique identifier of the group.</param>
    void BeginGroup(string key);

    /// <summary>
    /// End the execution of the current group.
    /// </summary>
    void EndGroup(object? result);
}