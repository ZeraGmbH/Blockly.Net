using System.Text.Json.Nodes;
using BlocklyNet.Scripting.Parsing;

namespace BlocklyNet.Extensions.Builder;

/// <summary>
/// 
/// </summary>
public class ConfigurationService : IConfigurationService
{
    /// <summary>
    /// The configuration will only be created once.
    /// </summary>
    private readonly JsonObject _cache;

    private readonly IScriptModels _models;

    /// <summary>
    /// Initialize a new controller. If the confuguration is not yet
    /// created this will be done - preferably once only. Since the code
    /// does not use explicit locking the configuration might be constructued
    /// more the once but this should not be a problem.
    /// </summary>
    public ConfigurationService(IScriptParser parser, IScriptModels models)
    {
        _models = models;

        /* Prepare the toolbox by grouping everything by category. */
        var toolbox = new JsonArray();

        var entries = parser
            .ToolboxEntries
            .GroupBy(e => e.Item1)
            .OrderBy(g => g.Key);

        foreach (var category in entries)
        {
            /* In each category provide an at least stable order using the unique block key. */
            var items = category.Select(c => c.Item2).OrderBy(i => (string)i["_name"]!);

            /* Entries without a category will be added on the top level of the toolbox. */
            if (string.IsNullOrEmpty(category.Key))
                foreach (var item in items)
                    toolbox.Add(item);
            else
            {
                /* Create a new category in the toolbox - nesting is not yet supported. */
                var isModels = category.Key == "*";

                var entry = new JsonObject
                {
                    ["kind"] = "category",
                    ["name"] = isModels ? "$Models$" : $"${category.Key}$",
                    ["contents"] = new JsonArray(items.ToArray())
                };

                toolbox.Add(entry);
            }
        }

        /* Create the configuration object. */
        _cache = new JsonObject
        {
            ["blocks"] = new JsonArray(parser.BlockDefinitions.ToArray()),
            ["models"] = new JsonArray(parser.ModelDefinitions.ToArray()),
            ["toolbox"] = new JsonArray(
                new JsonObject
                {
                    ["kind"] = "category",
                    ["name"] = "$CUSTOMROOT$",
                    ["contents"] = toolbox
                })
        };
    }

    /// <inheritdoc/>
    public JsonObject Configuration => _cache;

    /// <inheritdoc/>
    public IEnumerable<ScriptEngineModelInfo> ModelNames =>
        _models
            .Models
            .Select(e => new ScriptEngineModelInfo { Name = e.Value.Name, Type = e.Key })
            .OrderBy(i => i.Name, StringComparer.InvariantCultureIgnoreCase);

    /// <inheritdoc/>
    public IEnumerable<ScriptEngineModelInfo> EnumerationNames =>
        _models
            .Enums
            .Select(e => new ScriptEngineModelInfo { Name = e.Value.Name, Type = e.Key })
            .OrderBy(i => i.Name, StringComparer.InvariantCultureIgnoreCase);
}

