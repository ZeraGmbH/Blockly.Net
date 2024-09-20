using BlocklyNet.Core;
using BlocklyNet.Extensions.Builder;
using BlocklyNet.Scripting.Engine;
using Microsoft.Extensions.DependencyInjection;

namespace BlocklyNet;

/// <summary>
/// 
/// </summary>
public static class BlocklyDIExtensions
{
    /// <summary>
    /// Configure the dependency injection environment.
    /// </summary>
    /// <param name="services">Dependency injection services.</param>
    /// <param name="xml">Unset to use JSON serialization instead of the default XML.</param>
    public static void UseBlocklyNet(this IServiceCollection services, bool xml = true) => services.UseBlocklyNet<ScriptEngine>(xml);

    /// <summary>
    /// Configure the dependency injection environment.
    /// </summary>
    /// <typeparam name="TEngine">Engine implementation to use.</typeparam>
    /// <param name="services">Dependency injection services.</param>
    /// <param name="xml">Unset to use JSON serialization instead of the default XML.</param>
    public static void UseBlocklyNet<TEngine>(this IServiceCollection services, bool xml = true) where TEngine : ScriptEngine
    {
        /* Single management instance for all known models. */
        services.AddSingleton<IScriptModels, ScriptModels>();

        /* Blockly XML parser factory with optional custom configurator. */
        services.AddTransient(di => Parser.CreateService(xml, di.GetRequiredService<IScriptModels>(), di.GetService<IParserConfiguration>()));

        /* Script engine with optional notification connection. */
        services.AddSingleton<IScriptEngine, TEngine>();

        /* Overall access to the cached blocky configuration. */
        services.AddSingleton<IConfigurationService, ConfigurationService>();

        /* Group management algorithms. */
        services.AddTransient<IGroupManager, GroupManager>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    public static void StartBlocklyNet(this IServiceProvider services)
    {
        /* Start the engine on server startup to allow for timed script execution. */
        services.GetRequiredService<IScriptEngine>();
    }
}