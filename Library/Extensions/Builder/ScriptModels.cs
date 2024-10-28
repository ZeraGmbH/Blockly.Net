using System.Collections.Concurrent;

namespace BlocklyNet.Extensions.Builder;

/// <summary>
/// 
/// </summary>
public class ScriptModels : IScriptModels
{
    class ScriptModelInfo(string name, Type type, string? category) : IScriptModelInfo
    {
        public string Name { get; } = name;

        public Type Type { get; } = type;

        public string? Category { get; } = category;
    }

    /// <inheritdoc/>
    public ConcurrentDictionary<string, IScriptModelInfo> Models { get; } = [];

    /// <inheritdoc/>
    public ConcurrentDictionary<string, IScriptModelInfo> Enums { get; } = [];

    private static void Set(ConcurrentDictionary<string, IScriptModelInfo> map, string key, Type type, string name, string? category)
    {
        // Information to provide.
        var item = new ScriptModelInfo(name, type, category);

        // Use the newest one - actually should never happen.
        map.AddOrUpdate(key, (k) => item, (k, p) => item);
    }

    /// <inheritdoc/>
    public void SetEnum<T>(string key, string name, string? category) where T : Enum => Set(Enums, key, typeof(T), name, category);

    /// <inheritdoc/>
    public void SetModel(Type t, string key, string name, string? category) => Set(Models, key, t, name, category);
}
