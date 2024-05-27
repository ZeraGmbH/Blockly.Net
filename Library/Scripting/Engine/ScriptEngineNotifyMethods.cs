namespace BlocklyNet.Scripting.Engine;

public sealed class ScriptEngineNotifyMethods
{
    public readonly string Method;

    private ScriptEngineNotifyMethods(string method)
    {
        Method = method;
    }

    public static readonly ScriptEngineNotifyMethods Current = new("ActiveScript");

    public static readonly ScriptEngineNotifyMethods InputRequest = new("InputRequest");

    public static readonly ScriptEngineNotifyMethods Progress = new("ScriptProgress");

    public static readonly ScriptEngineNotifyMethods Started = new("ScriptStarted");

    public static readonly ScriptEngineNotifyMethods Done = new("ScriptDone");

    public static readonly ScriptEngineNotifyMethods Error = new("ScriptError");

    public static readonly ScriptEngineNotifyMethods Finished = new("ScriptFinished");
}
