namespace BlocklyNet.Scripting.Engine;

public interface IScriptEngineContext
{
    Task Broadcast(string method, object? arg1);
}
