using System.Text.Json.Nodes;

namespace BlocklyNet.Extensions.Builder;

public interface IConfigurationService
{
    JsonObject Configuration { get; }

    IEnumerable<ScriptEngineModelInfo> ModelNames { get; }

    IEnumerable<ScriptEngineModelInfo> EnumerationNames { get; }
}

