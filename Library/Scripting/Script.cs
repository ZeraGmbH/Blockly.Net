using BlocklyNet.Scripting.Engine;
using BlocklyNet.Scripting.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace BlocklyNet.Scripting;

/// <summary>
/// Official base class for implementing scripts.
/// </summary>
/// <typeparam name="TRequest">Type of the configuration.</typeparam>
/// <typeparam name="TResult">Result type of the script execution.</typeparam>
/// <typeparam name="TOption">Option class to use.</typeparam>
/// <typeparam name="TLogType">Type of each log entry.</typeparam>
/// <typeparam name="TModifierType">Implementation class of log modifiers.</typeparam>
public abstract class Script<TRequest, TResult, TOption, TLogType, TModifierType>(TRequest request, IScriptSite engine, TOption? options) : Script<TOption, TLogType, TModifierType>(options)
    where TRequest : StartScript
    where TOption : StartScriptOptions
    where TLogType : ScriptLoggingResult, new()
    where TModifierType : ScriptLogModifier
{
    /// <summary>
    /// Configuration of the script.
    /// </summary>
    public TRequest Request { get; } = request;

    /// <inheritdoc/>
    public void SetResult(TResult result) => Result = result;

    /// <inheritdoc/>
    public override StartScript GetRequest() => Request;

    /// <summary>
    /// Engine executing this script.
    /// </summary>
    public readonly IScriptSite Engine = engine;

    /// <summary>
    /// Test for cancel.
    /// </summary>
    protected void CheckCancel() => Engine.Cancellation.ThrowIfCancellationRequested();

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
