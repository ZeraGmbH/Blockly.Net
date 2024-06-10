namespace BlocklyNet.Scripting.Generic;

/// <summary>
/// 
/// </summary>
public interface IStartGenericScript
{
    /// <summary>
    /// Name of the script - the type of the script is defined by
    /// the type of derived classes.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// The script to execute.
    /// </summary>
    string ScriptId { get; set; }

    /// <summary>
    /// Type of the result.
    /// </summary>
    string? ResultType { get; set; }

    /// <summary>
    /// Parameters of the script.
    /// </summary>
    List<GenericScriptPreset> Presets { get; set; }
}