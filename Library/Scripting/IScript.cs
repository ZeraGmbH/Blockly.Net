using BlocklyNet.Scripting.Engine;
using BlocklyNet.Scripting.Logging;

namespace BlocklyNet.Scripting;

/// <summary>
/// Interface provided by any script.
/// </summary>
public interface IScript
{
    /// <summary>
    /// The unique identifier of the active script.
    /// </summary>
    string JobId { get; }

    /// <summary>
    /// Can be used to check for early termination.
    /// </summary>
    StartScriptOptions? Options { get; }

    /// <summary>
    /// Set group information of the script.
    /// </summary>
    /// <param name="status">Group information collected.</param>
    void SetGroups(ScriptGroupStatus? status);

    /// <summary>
    /// Report the related request.
    /// </summary>
    StartScript Request { get; }
}

/// <summary>
/// Type interface to a script.
/// </summary>
/// <typeparam name="TLogType">Type of a log entry.</typeparam>
public interface IScript<TLogType> : IScript where TLogType : ScriptLoggingResult, new()
{
    /// <summary>
    /// Logging entry for this script.
    /// </summary>
    TLogType ResultForLogging { get; }

    /// <summary>
    /// Set the outcome of the script.
    /// </summary>
    /// <param name="result">Outcome of the script.</param>
    Task SetResultAsync(ScriptExecutionResultTypes result);

    /// <summary>
    /// Update the current script status in the database.
    /// </summary>
    /// <returns>Unique identifier of the log record.</returns>
    Task<string> WriteToLogAsync();

    /// <summary>
    /// Register a child measurement.
    /// </summary>
    /// <param name="measurementId">Unique identifier of the child measurement.</param>
    Task RegisterChildAsync(string measurementId);
}