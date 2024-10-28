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
    /// <param name="modelType">.NET model type.</param>
    /// <param name="key">Blockly model type.</param>
    /// <param name="name">Display name of the model.</param>
    /// <param name="category">Category to use for the model.</param>
    void SetModel(Type modelType, string key, string name, string? category);

    /// <summary>
    /// Remember a enumeration block.
    /// </summary>
    /// <typeparam name="T">.NET enumeration type.</typeparam>
    /// <param name="key">Blockly model type.</param>
    /// <param name="name">Display name of the enumeration.</param>
    /// <param name="category">Category to use for the model.</param>
    void SetEnum<T>(string key, string name, string? category) where T : Enum;
}

/// <summary>
/// Helper methods for the IScriptModels interface.
/// </summary>
public static class IScriptModelsExtensions
{
    /// <summary>
    /// Remember a model block.
    /// </summary>
    /// <typeparam name="T">.NET model type.</typeparam>
    /// <param name="models">Interface to extend.</param>
    /// <param name="key">Blockly model type.</param>
    /// <param name="name">Display name of the model.</param>
    /// <param name="category">Category to use for the model.</param>
    public static void SetModel<T>(this IScriptModels models, string key, string name, string? category) where T : class
        => models.SetModel(typeof(T), key, name, category);
}