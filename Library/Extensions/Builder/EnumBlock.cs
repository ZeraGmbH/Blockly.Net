using System.Text.Json.Nodes;
using BlocklyNet.Core.Model;

namespace BlocklyNet.Extensions.Builder;

/// <summary>
/// Represents a script block for some enumerations.
/// </summary>
/// <remarks>
/// Not all type of properties are supported.
/// </remarks>
public class EnumBlock<T> : Block where T : Enum
{
    /// <summary>
    /// Block key of the model.
    /// </summary>
    private static string _key = null!;

    /// <summary>
    /// Display name of the model.
    /// </summary>
    private static string _name = null!;

    /// <summary>
    /// Initialize the static model generator data - this may be called more than once.
    /// </summary>
    /// <param name="key">Block key of the model.</param>
    /// <param name="name">Display name of the model.</param>
    public static Tuple<JsonObject, JsonObject> Initialize(string key, string name)
    {
        _key = key;
        _name = name;

        /* Generate the JSON descriptions of the model. */
        return Tuple.Create(CreateBlockDefinition(), CreateToolboxEntry());
    }

    /// <summary>
    /// Generate a block definition for the enumeration.
    /// </summary>
    /// <returns>The JSON description.</returns>
    private static JsonObject CreateBlockDefinition()
    {
        /* Generate the list from the enumeration values sorted by name. */
        var options = new List<string>();

        foreach (var value in Enum.GetValues(typeof(T)))
            options.Add(value.ToString()!);

        options.Sort();

        /* Create the selection. */
        var args = new JsonArray {
            new JsonObject
            {
                ["type"] = "field_dropdown",
                ["name"] = "VALUE",
                ["options"] = new JsonArray(options.Select(o => new JsonArray([o, o])).ToArray())
            }};

        /* Finish the block definition by putting it all together. */
        return new JsonObject
        {
            ["args0"] = args,
            ["message0"] = $"{_name} %1",
            ["inputsInline"] = true,
            ["output"] = _key,
            ["tooltip"] = _name,
            ["type"] = _key,
        };
    }

    /// <summary>
    /// Generate toolbox entry with default shadowed values.
    /// </summary>
    /// <returns>Desciption of a toolbox entry for this model.</returns>
    private static JsonObject CreateToolboxEntry()
    {
        /* Generate the toolbox entry in JSON format. */
        return new JsonObject
        {
            ["kind"] = "block",
            ["_name"] = _name,
            ["type"] = _key,
        };
    }

    /// <inheritdoc/>
    public override Task<object?> Evaluate(Context context)
    {
        /* Just report the value. */
        return Task.FromResult((object?)Enum.Parse(typeof(T), Fields["VALUE"]));
    }
}
