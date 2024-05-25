using System.Collections.Concurrent;

namespace BlocklyNet.Extensions.Builder;

/// <summary>
/// 
/// </summary>
public interface IScriptModels
{
    /// <summary>
    /// All known model blocks.
    /// </summary>
    ConcurrentDictionary<string, IScriptModelInfo> Models { get; }

    /// <summary>
    /// All known enumeration blocks.
    /// </summary>
    ConcurrentDictionary<string, IScriptModelInfo> Enums { get; }

    /// <summary>
    /// Remember a model block.
    /// </summary>
    /// <typeparam name="T">.NET model type.</typeparam>
    /// <param name="key">Blockly model type.</param>
    /// <param name="name">Display name of the model.</param>
    void SetModel<T>(string key, string name) where T : class;

    /// <summary>
    /// Remember a enumeration block.
    /// </summary>
    /// <typeparam name="T">.NET enumeration type.</typeparam>
    /// <param name="key">Blockly model type.</param>
    /// <param name="name">Display name of the enumeration.</param>
    void SetEnum<T>(string key, string name) where T : Enum;
}
