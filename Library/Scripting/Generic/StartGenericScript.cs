using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlocklyNet.Scripting.Generic;

/// <summary>
/// 
/// </summary>
public interface IStartGenericScript
{
    /// <summary>
    /// The script to execute.
    /// </summary>
    string ScriptId { get; }

    /// <summary>
    /// Type of the result.
    /// </summary>
    string? ResultType { get; }

    /// <summary>
    /// Parameters of the script.
    /// </summary>
    List<GenericScriptPreset> Presets { get; set; }
}

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
public class StartGenericScript : StartGenericScript<GenericScript, GenericResult>
{
}