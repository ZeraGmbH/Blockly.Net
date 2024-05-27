using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlocklyNet.Scripting.Generic;

/// <summary>
/// Start a generic script based on the integrated script language.
/// </summary>
public class StartGenericScript<TScript, TResult> : StartScript<TScript, TResult> where TScript : GenericScript where TResult : GenericResult
{
    /// <summary>
    /// Parameters for the script.
    /// </summary>
    public const string ScriptType = "Generic";

    /// <inheritdoc/>
    public override string ModelType => ScriptType;

    /// <summary>
    /// Parameters of the script.
    /// </summary>
    [NotNull, Required]
    public List<GenericScriptPreset> Presets { get; set; } = [];

    /// <summary>
    /// The script to execute.
    /// </summary>
    [Required, NotNull]
    public string ScriptId { get; set; } = null!;

    /// <summary>
    /// Type of the result.
    /// </summary>
    public string? ResultType { get; set; }
}

/// <summary>
/// Start a generic script based on the integrated script language.
/// </summary>
public class StartGenericScript : StartGenericScript<GenericScript, GenericResult>
{
}