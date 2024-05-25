namespace BlocklyNet.Scripting;

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
    Task Execute();

    /// <summary>
    /// 
    /// </summary>
    StartScript GetRequest();
}
