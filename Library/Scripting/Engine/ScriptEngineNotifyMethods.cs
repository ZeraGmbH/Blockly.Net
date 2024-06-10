namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// 
/// </summary>
public sealed class ScriptEngineNotifyMethods
{
    /// <summary>
    /// 
    /// </summary>
    public readonly string Method;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="method"></param>
    private ScriptEngineNotifyMethods(string method)
    {
        Method = method;
    }

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
