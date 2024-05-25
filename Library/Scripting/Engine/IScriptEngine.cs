using BlocklyNet.Scripting.Parsing;

namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// 
/// </summary>
public interface IScriptEngine
{
    /// <summary>
    /// Start the indicated script.
    /// </summary>
    /// <param name="request">Configuration of the script run.</param>
    /// <param name="userToken">Totken used by a user to identify himself.</param>
    /// <param name="options">Further configuration of the script.</param>
    string Start(StartScript request, string userToken, StartScriptOptions? options = null);

    /// <summary>
    /// Abort the currently running script.
    /// <param name="jobId"></param>
    /// </summary>
    void Cancel(string jobId);

    /// <summary>
    /// Get the script result and forget about the execution.
    /// </summary>
    /// <param name="jobId">The already finished script.</param>
    /// <returns>Result of the script.</returns>
    object? FinishScriptAndGetResult(string jobId);

    /// <summary>
    /// A client reports a requested value.
    /// </summary>
    /// <param name="value">Information on the request and the value.</param>
    void SetUserInput(UserInputResponse value);

    /// <summary>
    /// Report the script parsing engine to use.
    /// </summary>
    IScriptParser Parser { get; }
}
