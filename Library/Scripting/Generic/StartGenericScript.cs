using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using BlocklyNet.Scripting.Logging;

namespace BlocklyNet.Scripting.Generic;

/// <summary>
/// Start a generic script based on the integrated script language.
/// </summary>
public class StartGenericScript<TScript, TResult> : StartScript<TScript, TResult>, IStartGenericScript where TScript : Script where TResult : GenericResult
{
    /// <summary>
    /// Parameters for the script.
    /// </summary>
    public const string ScriptType = "Generic";

    /// <inheritdoc/>
    public override string ModelType => ScriptType;

    /// <inheritdoc/>
    [NotNull, Required]
    public List<GenericScriptPreset> Presets { get; set; } = [];

    /// <inheritdoc/>
    [Required, NotNull]
    public string ScriptId { get; set; } = null!;

    /// <inheritdoc/>
    public string? ResultType { get; set; }
}

/// <summary>
/// Start a generic script based on the integrated script language.
/// </summary>
public class StartGenericScript : StartGenericScript<GenericScript<ScriptLoggingResult, StartGenericScript.NoopModifier>, GenericResult>
{
    /// <summary>
    /// 
    /// </summary>
    public class NoopModifier : IScriptLogModifier
    {
        /// <inheritdoc/>
        public Task ApplyAsync(Script script, bool initial) => Task.CompletedTask;
    }
}