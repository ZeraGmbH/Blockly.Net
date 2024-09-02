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
