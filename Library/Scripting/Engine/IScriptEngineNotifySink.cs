namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// 
/// </summary>
public interface IScriptEngineNotifySink
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="method"></param>
    /// <param name="arg1"></param>
    /// <returns></returns>
    Task Send(ScriptEngineNotifyMethods method, object? arg1);
}
