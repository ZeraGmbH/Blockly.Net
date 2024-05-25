using System.Collections.Concurrent;

namespace BlocklyNet.Extensions.Builder;

/// <summary>
/// 
/// </summary>
public class ScriptModels : IScriptModels
{
    class ScriptModelInfo(string name, Type type) : IScriptModelInfo
    {
        public string Name { get; } = name;

        public Type Type { get; } = type;
    }

    /// <inheritdoc/>
    public ConcurrentDictionary<string, IScriptModelInfo> Models { get; } = [];

    /// <inheritdoc/>
    public ConcurrentDictionary<string, IScriptModelInfo> Enums { get; } = [];

    private static void Set(ConcurrentDictionary<string, IScriptModelInfo> map, string key, Type type, string name) =>
        map.AddOrUpdate(key, (k) => new ScriptModelInfo(name, type), (k, p) => new ScriptModelInfo(name, type));

    /// <inheritdoc/>
    public void SetEnum<T>(string key, string name) where T : Enum => Set(Enums, key, typeof(T), name);

    /// <inheritdoc/>
    public void SetModel<T>(string key, string name) where T : class => Set(Models, key, typeof(T), name);
}
