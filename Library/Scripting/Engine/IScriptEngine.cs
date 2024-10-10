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
    Task<string> StartAsync(StartScript request, string userToken, StartScriptOptions? options = null);

    /// <summary>
    /// Abort the currently running script.
    /// <param name="jobId"></param>
    /// </summary>
    void Cancel(string jobId);

    /// <summary>
    /// Request to pause the script on the next execution group end.
    /// </summary>
    /// <param name="jobId"></param>
    void Pause(string jobId);

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
    void SetUserInput(UserInputResponse? value);

    /// <summary>
    /// Report the script parsing engine to use.
    /// </summary>
    IScriptParser Parser { get; }

    /// <summary>
    /// Send current status to a newly connected client to let it keep up.
    /// </summary>
    /// <param name="client">The new client.</param>
    void Reconnect(IScriptEngineNotifySink client);

    /// <summary>
    /// Use the group manager to create flat result list.
    /// </summary>
    /// <returns>Unset if no groups are used.</returns>
    List<object?>? CreateFlatResultFromGroups();

    /// <summary>
    /// Analyse the script code in fitting string representation
    /// and report all information on the group structure.
    /// </summary>
    /// <param name="code">Some script.</param>
    /// <returns>Group structure of the script.</returns>
    Task<List<GroupInfo>> GetGroupsForScriptAsync(string code);
}
