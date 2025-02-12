using BlocklyNet.Scripting.Generic;

namespace BlocklyNet.Scripting.Engine;

partial class ScriptEngine<TLogType>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="script"></param>
    /// <returns></returns>
    protected virtual ScriptInformation CreateStartNotification(IScriptInstance script)
        => new()
        {
            JobId = script.JobId,
            ModelType = script.GetRequest().ModelType,
            Name = script.GetRequest().Name,
            ScriptId = script.GetRequest() is IStartGenericScript generic ? generic.ScriptId : null,
        };

    /// <summary>
    /// 
    /// </summary>
    /// <param name="script"></param>
    /// <returns></returns>
    protected virtual ScriptInformation CreateCurrentNotification(IScriptInstance script)
        => new()
        {
            JobId = script.JobId,
            ModelType = script.GetRequest().ModelType,
            Name = script.GetRequest().Name,
            ScriptId = script.GetRequest() is IStartGenericScript generic ? generic.ScriptId : null,
        };

    /// <summary>
    /// 
    /// </summary>
    /// <param name="script"></param>
    /// <returns></returns>
    protected virtual ScriptDone CreateDoneNotification(IScriptInstance script)
        => new()
        {
            GroupStatus = SerializeGroupStatus(true),
            JobId = script.JobId,
            ModelType = script.GetRequest().ModelType,
            Name = script.GetRequest().Name,
            ScriptId = script.GetRequest() is IStartGenericScript generic ? generic.ScriptId : null,
        };

    /// <summary>
    /// 
    /// </summary>
    /// <param name="script"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    protected virtual ScriptError CreateErrorNotification(IScriptInstance script, Exception error)
        => new()
        {
            ErrorMessage = error.Message,
            GroupStatus = SerializeGroupStatus(true),
            JobId = script.JobId,
            ModelType = script.GetRequest().ModelType,
            Name = script.GetRequest().Name,
            ScriptId = script.GetRequest() is IStartGenericScript generic ? generic.ScriptId : null,
        };

    /// <summary>
    /// 
    /// </summary>
    /// <param name="script"></param>
    /// <returns></returns>
    protected virtual ScriptFinished CreateFinishNotification(IScriptInstance script)
        => new()
        {
            GroupStatus = SerializeGroupStatus(true),
            JobId = script.JobId,
            ModelType = script.GetRequest().ModelType,
            Name = script.GetRequest().Name,
            ScriptId = script.GetRequest() is IStartGenericScript generic ? generic.ScriptId : null,
        };
}
