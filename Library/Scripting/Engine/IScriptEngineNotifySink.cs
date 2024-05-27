namespace BlocklyNet.Scripting.Engine;

public interface IScriptEngineNotifySink
{
    Task Send(ScriptEngineNotifyMethods method, object? arg1);
}
