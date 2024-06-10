using System.Text.Json.Nodes;

namespace BlocklyNet.Extensions.Builder;

/// <summary>
/// 
/// </summary>
public interface IConfigurationService
{
    /// <summary>
    /// 
    /// </summary>
    JsonObject Configuration { get; }

    /// <summary>
    /// 
    /// </summary>
    IEnumerable<ScriptEngineModelInfo> ModelNames { get; }

    /// <summary>
    /// 
    /// </summary>
    IEnumerable<ScriptEngineModelInfo> EnumerationNames { get; }
}

