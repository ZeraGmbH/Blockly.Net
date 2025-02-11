using BlocklyNet.Scripting.Logging;

namespace BlocklyNet.Scripting;

/// <summary>
/// Describes an active script.
/// </summary>
public abstract class Script<TOption, TLogType> : Script
    where TOption : StartScriptOptions
    where TLogType : ScriptLoggingResult, new()
{
    private readonly TOption? _options;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    public Script(TOption? options)
    {
        _options = options;

        ResultForLogging = CreateResultForLogging();
    }

    /// <inheritdoc/>
    public override TOption? Options => _options;

    /// <summary>
    /// 
    /// </summary>
    public TLogType ResultForLogging { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected virtual TLogType CreateResultForLogging() => new();

    /// <summary>
    /// Internal reset of state.
    /// </summary>
    protected override Task ReinitializeAsync()
    {
        ResultForLogging = CreateResultForLogging();

        return base.ReinitializeAsync();
    }
}
