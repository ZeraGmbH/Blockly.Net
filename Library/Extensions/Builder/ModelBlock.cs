using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using BlocklyNet.Core.Model;
using Newtonsoft.Json;

namespace BlocklyNet.Extensions.Builder;

/// <summary>
/// Represents a script block for some model.
/// </summary>
/// <remarks>
/// Not all type of properties are supported.
/// </remarks>
public class ModelBlock<T> : Block where T : class, new()
{
    /// <summary>
    /// Description of a well known type.
    /// </summary>
    /// <param name="typeName">Script name of the type.</param>
    /// <param name="blockType">Block to initialize a value of the type.</param>
    /// <param name="constProp">Block value holding the value.</param>
    /// <param name="constValue">Default value in text representation.</param>
    private class TypeInformation(string typeName, string blockType, string constProp, string constValue)
    {
        /// <summary>
        /// Script name of the type.
        /// </summary>
        public readonly string TypeName = typeName;

        /// <summary>
        /// Block to initialize a value of the type.
        /// </summary>
        public readonly string BlockType = blockType;

        /// <summary>
        /// Block value holding the value.
        /// </summary>
        public readonly string ConstProp = constProp;

        /// <summary>
        /// Default value in text representation.
        /// </summary>
        public readonly string ConstValue = constValue;
    }

    /// <summary>
    /// All wellknown elementary types supported.
    /// </summary>
    private static readonly Dictionary<Type, TypeInformation> _supportedTypes = new(){
        /* Boolean. */
        {typeof(bool), new("Boolean","logic_boolean","BOOL","FALSE")},
        {typeof(bool?), new("Boolean","logic_boolean","BOOL","FALSE")},
        /* Numbers. */
        {typeof(double), new("Number","math_number","NUM","0")},
        {typeof(double?), new("Number","math_number","NUM","0")},
        {typeof(int), new("Number","math_number","NUM","0")},
        {typeof(int?), new("Number","math_number","NUM","0")},
        /* String. */
        {typeof(string), new("String","text","TEXT","")},
    };

    /// <summary>
    /// List of supported properties of the related type.
    /// </summary>
    private static PropertyInfo[] _props = null!;

    /// <summary>
    /// Block key of the model.
    /// </summary>
    private static string _key = null!;

    /// <summary>
    /// Display name of the model.
    /// </summary>
    private static string _name = null!;

    /// <summary>
    /// See if some type is supported by the generic model generator.
    /// </summary>
    /// <param name="type">Some type of a property of the model.</param>
    /// <param name="models">All other known models.</param>
    /// <returns>Set if the type is supported.</returns>
    private static bool TestSupported(Type type, Dictionary<Type, string> models)
    {
        // Enums are always good.
        if (type.IsEnum) return true;

        // Some of .NET base types.
        if (_supportedTypes.ContainsKey(type)) return true;

        // Already known other model.
        if (models.ContainsKey(type)) return true;

        return false;
    }

    /// <summary>
    /// Get the element type of a list ist the type is such a list.
    /// </summary>
    /// <param name="type">Property type to validate.</param>
    /// <returns>Set to the element type if the property type is a generic typed list.</returns>
    private static Type? TestArray(Type type)
    {
        // Not generic at all.
        if (!type.IsGenericType) return null;

        // Only List<> is supported.
        if (type.GetGenericTypeDefinition() != typeof(List<>)) return null;

        // Report the inner type
        return type.GenericTypeArguments[0];
    }

    /// <summary>
    /// Initialize the static model generator data - this may be called more than once.
    /// </summary>
    /// <param name="key">Block key of the model.</param>
    /// <param name="name">Display name of the model.</param>
    /// <param name="models">Other models already registered.</param>
    public static Tuple<JsonObject, JsonObject> Initialize(string key, string name, Dictionary<Type, string> models)
    {
        _key = key;
        _name = name;

        /* Get all properties of supported types - including a generic list of supported types. */
        _props = typeof(T)
            .GetProperties()
            .Where(p => TestSupported(TestArray(p.PropertyType) ?? p.PropertyType, models))
            .Where(p => p.GetCustomAttribute<JsonIgnoreAttribute>() == null && p.GetCustomAttribute<System.Text.Json.Serialization.JsonIgnoreAttribute>() == null)
            .ToArray();

        /* Generate the JSON descriptions of the model. */
        return Tuple.Create(CreateBlockDefinition(models), CreateToolboxEntry(models));
    }

    /// <summary>
    /// Generate a block definition for the model.
    /// </summary>
    /// <param name="models">All models already registered.</param>
    /// <returns>The JSON description.</returns>
    private static JsonObject CreateBlockDefinition(Dictionary<Type, string> models)
    {
        var args = new JsonArray { new JsonObject { ["type"] = "input_dummy" } };

        /* Process all properties known as supported. */
        foreach (var prop in _props)
        {
            /* Always generate a label. */
            args.Add(new JsonObject
            {
                ["type"] = "field_label_serializable",
                ["name"] = prop.Name,
                ["text"] = prop.Name
            });

            /* For enumerations provide a static selection list. */
            if (prop.PropertyType.IsEnum)
            {
                /* Just add the reference. */
                args.Add(new JsonObject
                {
                    ["type"] = "input_value",
                    ["name"] = prop.Name,
                    ["check"] = models[prop.PropertyType]
                });

                continue;
            }

            /* See if the property is a list. */
            var elementType = TestArray(prop.PropertyType);

            if (elementType != null)
            {
                /* Type of each array element. */
                var itemType = _supportedTypes.TryGetValue(elementType, out var elem)
                    ? elem.TypeName
                    : models[elementType];

                /* Type checkings end at the array level - no element type validation possible. */
                args.Add(new JsonObject
                {
                    ["type"] = "input_value",
                    ["name"] = prop.Name,
                    ["check"] = new JsonArray($"Array({itemType})", "Array")
                });

                continue;
            }

            /* According to the preparation we either have some wellknown type or another model. */
            var knownType = _supportedTypes.TryGetValue(prop.PropertyType, out var info)
                ? info.TypeName
                : models[prop.PropertyType];

            /* Generate the related input. */
            args.Add(new JsonObject
            {
                ["type"] = "input_value",
                ["name"] = prop.Name,
                ["check"] = knownType
            });
        }

        /* Build a message string from our arguments. */
        var message = new StringBuilder(_name);

        for (var i = 0; i++ < args.Count;)
        {
            message.Append(" %");
            message.Append(i);
        }

        /* Finish the block definition by putting it all together. */
        return new JsonObject
        {
            ["args0"] = args,
            ["message0"] = message.ToString(),
            ["output"] = _key,
            ["tooltip"] = _name,
            ["type"] = _key,
        };
    }

    /// <summary>
    /// Generate toolbox entry with default shadowed values.
    /// </summary>
    /// <param name="models">Other models already registered.</param>
    /// <returns>Desciption of a toolbox entry for this model.</returns>
    private static JsonObject CreateToolboxEntry(Dictionary<Type, string> models)
    {
        var inputs = new JsonObject();

        /* For all wellknown types we can provide a default value using a shadow block. */
        foreach (var prop in _props)
            if (prop.PropertyType.IsEnum)
                inputs[prop.Name] = new JsonObject
                {
                    ["shadow"] = new JsonObject
                    {
                        ["type"] = models[prop.PropertyType],
                        ["fields"] = new JsonObject { ["VALUE"] = Enum.GetValues(prop.PropertyType).GetValue(0)!.ToString() }
                    }
                };
            else if (_supportedTypes.TryGetValue(prop.PropertyType, out var info))
                inputs[prop.Name] = new JsonObject
                {
                    ["shadow"] = new JsonObject
                    {
                        ["type"] = info.BlockType,
                        ["fields"] = new JsonObject { [info.ConstProp] = info.ConstValue }
                    }
                };

        /* Generate the toolbox entry in JSON format. */
        return new JsonObject
        {
            ["inputs"] = inputs,
            ["kind"] = "block",
            ["_name"] = _name,
            ["type"] = _key,
        };
    }

    /// <inheritdoc/>
    public override async Task<object?> Evaluate(Context context)
    {
        /* Create a new model instance. */
        var model = new T();

        /* Populate all supported properties. */
        foreach (var prop in _props)
        {
            /* Get the raw value from the script. This may be untyped which especially for lists can be quite a problem. */
            var blocklyData = await Values.Evaluate(prop.Name, context, false);

            if (blocklyData == null) continue;

            /* Too make sure that the data fits run it to a serialize/deserialze sequence - currently performance should not be a problem. */
            var dataAsJson = JsonConvert.SerializeObject(blocklyData);
            var typedData = JsonConvert.DeserializeObject(dataAsJson, prop.PropertyType);

            /* Store the adapted value in the model. */
            prop.SetValue(model, typedData);
        }

        return model;
    }
}
