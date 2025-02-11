using BlocklyNet.Scripting.Engine;
using BlocklyNet.Scripting.Logging;

namespace BlocklyNet.Scripting;

/// <summary>
/// 
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
/// 
/// </summary>
/// <typeparam name="TLogType"></typeparam>
public interface IScript<TLogType> : IScript where TLogType : ScriptLoggingResult, new()
{
    /// <summary>
    /// 
    /// </summary>
    TLogType ResultForLogging { get; }

    /// <summary>
    /// Set the outcome of the script.
    /// </summary>
    /// <param name="result">Outcome of the script.</param>
    Task SetResultAsync(ScriptExecutionResultTypes result);
}