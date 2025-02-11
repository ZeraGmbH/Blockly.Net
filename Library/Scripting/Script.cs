using BlocklyNet.Core.Model;
using BlocklyNet.Scripting.Engine;
using BlocklyNet.Scripting.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace BlocklyNet.Scripting;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResult"></typeparam>
/// <typeparam name="TOption"></typeparam>
/// <typeparam name="TLogType"></typeparam>
/// <typeparam name="TModifierType"></typeparam>
public abstract class Script<TRequest, TResult, TOption, TLogType, TModifierType>(TRequest request, IScriptSite engine, TOption? options) : Script<TOption, TLogType, TModifierType>(options)
    where TRequest : StartScript
    where TOption : StartScriptOptions
    where TLogType : ScriptLoggingResult, new()
    where TModifierType : IScriptLogModifier
{
    /// <summary>
    /// 
    /// </summary>
    public TRequest Request { get; } = request;

    /// <inheritdoc/>
    public void SetResult(TResult result) => Result = result;

    /// <summary>
    /// 
    /// </summary>
    public override StartScript GetRequest() => Request;

    /// <summary>
    /// 
    /// </summary>
    public readonly IScriptSite Engine = engine;

    /// <summary>
    /// Test for cancel.
    /// </summary>
    protected void CheckCancel()
    {
        // Brute force cancel.
        Engine.Cancellation.ThrowIfCancellationRequested();

        // More soft pause.
        if (Engine.MustPause) throw new ScriptPausedException();
    }

    /// <summary>
    /// Access a runtime service.
    /// </summary>
    /// <typeparam name="T">Type of the service.</typeparam>
    /// <returns>Service as requested.</returns>
    protected T GetService<T>() where T : notnull => Engine.ServiceProvider.GetRequiredService<T>();

    /// <summary>
    /// Execute the script.
    /// </summary>
    protected abstract Task OnExecuteAsync();

    /// <inheritdoc/>
    public sealed override Task ExecuteAsync() => OnExecuteAsync();
}
