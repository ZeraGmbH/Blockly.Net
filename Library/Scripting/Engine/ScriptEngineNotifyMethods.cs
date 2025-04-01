namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// 
/// </summary>
/// <param name="method"></param>
public sealed class ScriptEngineNotifyMethods(string method)
{
    /// <summary>
    /// 
    /// </summary>
    public readonly string Method = method;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static readonly ScriptEngineNotifyMethods Current = new("ActiveScript");

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static readonly ScriptEngineNotifyMethods InputRequest = new("InputRequest");

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static readonly ScriptEngineNotifyMethods Progress = new("ScriptProgress");

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static readonly ScriptEngineNotifyMethods Started = new("ScriptStarted");

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static readonly ScriptEngineNotifyMethods Done = new("ScriptDone");

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static readonly ScriptEngineNotifyMethods Error = new("ScriptError");

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static readonly ScriptEngineNotifyMethods Finished = new("ScriptFinished");
}
