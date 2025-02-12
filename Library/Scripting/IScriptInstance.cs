using BlocklyNet.Scripting.Logging;

namespace BlocklyNet.Scripting;

/// <summary>
/// 
/// </summary>
public interface IScriptInstance : IScript
{
    /// <summary>
    /// Untyped result of the script.
    /// </summary>
    object? Result { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task ExecuteAsync();

    /// <summary>
    /// 
    /// </summary>
    StartScript GetRequest();
}

/// <summary>
/// 
/// </summary>
public interface IScriptInstance<TLogType> : IScript<TLogType>, IScriptInstance where TLogType : ScriptLoggingResult, new()
{
}
