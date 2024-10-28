using System.Collections;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using BlocklyNet.Core.Model;

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
    /// Abstract access to a property.
    /// </summary>
    /// <remarks>
    /// Initialize manually.
    /// </remarks>
    /// <param name="name">Name of the property.</param>
    /// <param name="type">Type of the value.</param>
    /// <param name="setValue">Method to set values.</param>
    private class PropertyInformation(string name, Type type, Action<object, object?> setValue)
    {
        /// <summary>
        /// Name of the property.
        /// </summary>
        public readonly string Name = name;

        /// <summary>
        /// Type of the property value.
        /// </summary>
        public readonly Type Type = type;

        /// <summary>
        /// Set the value of the property.
        /// </summary>
        public readonly Action<object, object?> SetValue = setValue;

        /// <summary>
        /// Initialize from reflection.
        /// </summary>
        /// <param name="info">Property information.</param>
        public PropertyInformation(PropertyInfo info) : this(info.Name, info.PropertyType, info.SetValue) { }
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
        {typeof(long), new("Number","math_number","NUM","0")},
        {typeof(long?), new("Number","math_number","NUM","0")},
        {typeof(int), new("Number","math_number","NUM","0")},
        {typeof(int?), new("Number","math_number","NUM","0")},
        /* String. */
        {typeof(string), new("String","text","TEXT","")},
        /* Object. */
        {typeof(object), new("","","","")},
    };

    /// <summary>
    /// List of supported properties of the related type.
    /// </summary>
    private static PropertyInformation[] _props = null!;

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
    /// <param name="category">Toolbox category to use.</param>
    /// <param name="models">All other known models.</param>
    /// <param name="modelFactory">Callback to create models on the fly, e.g. dictionaries.</param>
    /// <param name="propertyName">Name of the related property.</param>
    /// <returns>Set if the type is supported.</returns>
    private static bool TestSupported(Type type, string? category, ModelCache models, Func<Type, string, string, string?, bool> modelFactory, string propertyName)
    {
        // Enums are always good.
        if (type.IsEnum && models.Contains(type)) return true;

        // Some of .NET base types.
        if (_supportedTypes.ContainsKey(type)) return true;

        // Already known other model.
        if (models.Contains(type)) return true;

        return modelFactory(type, $"{_key}_{propertyName}", _name, category);
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
    /// <param name="category">Toolbox category to use.</param>
    /// <param name="models">Other models already registered.</param>
    /// <param name="modelFactory">Callback to create models on the fly, e.g. dictionaries.</param>
    public static Tuple<JsonObject, JsonObject> Initialize(string key, string name, string? category, ModelCache models, Func<Type, string, string, string?, bool> modelFactory)
    {
        _key = key;
        _name = name;
        _props = null!;

        if (typeof(T).IsGenericType)
            /* Special handling of dictionaries. */
            if (typeof(T).GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                /* Key must be a known enumeration. */
                var keyType = typeof(T).GetGenericArguments()[0];

                if (keyType.IsEnum && models.Contains(keyType))
                {
                    /* Value type must be supported. */
                    var valueType = typeof(T).GetGenericArguments()[1];

                    if (TestSupported(TestArray(valueType) ?? valueType, category, models, modelFactory, "Value"))
                        _props = Enum
                            .GetValues(keyType)
                            .Cast<object>()
                            .Select(key => new PropertyInformation(key.ToString()!, valueType, (obj, value) => ((IDictionary)obj)[key] = value))
                            .ToArray();
                }
            }
            /* Special handling of lists. */
            else if (typeof(T).GetGenericTypeDefinition() == typeof(List<>))
                _props ??= typeof(T)
                    .GetProperties()
                    .Where(p => p.Name != nameof(List<object>.Capacity))
                    .Where(p => p.GetCustomAttribute<JsonIgnoreAttribute>() == null && p.CanRead && p.CanWrite)
                    .Where(p => TestSupported(TestArray(p.PropertyType) ?? p.PropertyType, category, models, modelFactory, p.Name))
                    .Select(p => new PropertyInformation(p))
                    .ToArray();

        /* Get all properties of supported types - including a generic list of supported types. */
        _props ??= typeof(T)
                .GetProperties()
                .Where(p => p.GetCustomAttribute<JsonIgnoreAttribute>() == null && p.CanRead && p.CanWrite)
                .Where(p => TestSupported(TestArray(p.PropertyType) ?? p.PropertyType, category, models, modelFactory, p.Name))
                .Select(p => new PropertyInformation(p))
                .ToArray();

        /* Generate the JSON descriptions of the model. */
        return Tuple.Create(CreateBlockDefinition(models), CreateToolboxEntry(models));
    }

    /// <summary>
    /// Generate a block definition for the model.
    /// </summary>
    /// <param name="models">All models already registered.</param>
    /// <returns>The JSON description.</returns>
    private static JsonObject CreateBlockDefinition(ModelCache models)
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
            if (prop.Type.IsEnum)
            {
                /* Just add the reference. */
                args.Add(new JsonObject
                {
                    ["type"] = "input_value",
                    ["name"] = prop.Name,
                    ["check"] = models[prop.Type]
                });

                continue;
            }

            /* See if the property is a list. */
            var elementType = TestArray(prop.Type);

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
                    ["check"] = string.IsNullOrEmpty(itemType) ? "Array" : new JsonArray($"Array({itemType})", "Array")
                });

                continue;
            }

            /* According to the preparation we either have some wellknown type or another model. */
            var knownType = _supportedTypes.TryGetValue(prop.Type, out var info)
                ? info.TypeName
                : models[prop.Type];

            /* Generate the related input. */
            var input = new JsonObject
            {
                ["type"] = "input_value",
                ["name"] = prop.Name,
            };

            if (!string.IsNullOrEmpty(knownType)) input["check"] = knownType;

            args.Add(input);
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
    private static JsonObject CreateToolboxEntry(ModelCache models)
    {
        var inputs = new JsonObject();

        /* For all wellknown types we can provide a default value using a shadow block. */
        foreach (var prop in _props)
            if (prop.Type.IsEnum)
                inputs[prop.Name] = new JsonObject
                {
                    ["shadow"] = new JsonObject
                    {
                        ["type"] = models[prop.Type],
                        ["fields"] = new JsonObject { ["VALUE"] = Enum.GetValues(prop.Type).GetValue(0)!.ToString() }
                    }
                };
            else if (_supportedTypes.TryGetValue(prop.Type, out var info) && !string.IsNullOrEmpty(info.BlockType))
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
    public override async Task<object?> EvaluateAsync(Context context)
    {
        /* Create a new model instance. */
        var model = new T();

        /* Populate all supported properties. */
        foreach (var prop in _props)
        {
            /* Get the raw value from the script. This may be untyped which especially for lists can be quite a problem. */
            var blocklyData = await Values.EvaluateAsync(prop.Name, context, false);

            if (blocklyData == null) continue;

            /* Too make sure that the data fits run it to a serialize/deserialze sequence - currently performance should not be a problem. */
            var dataAsJson = JsonSerializer.Serialize(blocklyData, JsonUtils.JsonSettings);
            var typedData = JsonSerializer.Deserialize(dataAsJson, prop.Type, JsonUtils.JsonSettings);

            /* Store the adapted value in the model. */
            prop.SetValue(model, typedData);
        }

        return model;
    }
}
