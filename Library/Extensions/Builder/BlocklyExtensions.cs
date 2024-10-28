using BlocklyNet.Core;
using BlocklyNet.Core.Blocks;
using BlocklyNet.Core.Model;
using BlocklyNet.Scripting.Engine;
using System.Reflection;
using System.Text.Json.Nodes;

namespace BlocklyNet.Extensions.Builder;

/// <summary>
/// Configuration methods for script parsers.
/// </summary>
public static class BlocklyExtensions
{
    /// <summary>
    /// Tag class to create typed configuration helper.
    /// </summary>
    static class BlockBuilder
    {
        /// <summary>
        /// Create a txped configuration helper.
        /// </summary>
        /// <typeparam name="TParser">Type of the parser to use.</typeparam>
        /// <param name="parser">Parser to use.</param>
        /// <param name="models">Block model manager.</param>
        /// <returns>Parser configuration helper instance.</returns>
        public static BlockBuilder<TParser> Create<TParser>(Parser<TParser> parser, IScriptModels models) where TParser : Parser<TParser> => new(parser, models);
    }

    /// <summary>
    /// Parser configuration class.
    /// </summary>
    /// <typeparam name="TParser">Type of the parser to use.</typeparam>
    /// <param name="parser">Parser to use.</param>
    /// <param name="models">Block model manager.</param>
    public class BlockBuilder<TParser>(Parser<TParser> parser, IScriptModels models) where TParser : Parser<TParser>
    {
        /// <summary>
        /// All models defined so far.
        /// </summary>
        private readonly ModelCache _models = new();

        /// <summary>
        /// The parser to configure.
        /// </summary>
        private readonly Parser<TParser> _parser = parser;

        /// <summary>
        /// Generate all blockly definitions including the toolbox entries.
        /// </summary>
        public void CreateDefinitions() => _models.CreateDefinitions();

        /// <summary>
        /// Add a self describing block class to the parser.
        /// </summary>
        /// <param name="registerAs">Set if this block should be handled like a model.</param>
        /// <param name="category">Toolbox category to use.</param>
        /// <typeparam name="TBlock">Type of the block.</typeparam>
        public void AddBlock<TBlock>(Type? registerAs = null, string? category = null) where TBlock : Block, new()
        {
            /* Attribute indicator is required. */
            var blockAttribute = typeof(TBlock).GetCustomAttributes<CustomBlockAttribute>().Single();

            /* Add the block type using the unique block key from the attribute. */
            var key = blockAttribute.Key;

            _parser.AddBlock<TBlock>(key);

            /* Block definition is required as well. */
            var blockDefinition = blockAttribute.Definition;

            /* Convert to a JSON object, append the block key and remember in parser. */
            var blockJson = (JsonObject)JsonNode.Parse(blockDefinition)!;

            blockJson["type"] = key;

            _parser.BlockDefinitions.Add(blockJson);

            /* A toolbox entry is always provided but it can be extended using the attribute, */
            var toolbox =
             string.IsNullOrEmpty(blockAttribute.Toolbox)
                ? []
                : (JsonObject)JsonNode.Parse(blockAttribute.Toolbox)!;

            /* Fill in the core data of the toolbox entry and report it to the parser. */
            var message0 = (string)blockJson["message0"]!;
            var part0 = message0?.Split(" ").FirstOrDefault(key);

            toolbox["kind"] = "block";
            toolbox["_name"] = part0;
            toolbox["type"] = key;

            _parser.ToolboxEntries.Add(Tuple.Create(blockAttribute.Category, toolbox));

            /* Remember the model type - future models can now reference the property type. */
            if (registerAs == null) return;

            _models.Add(registerAs, key);

            /* For value types add a nullable as well. */
            if (registerAs.IsValueType) _models.Add(typeof(Nullable<>).MakeGenericType(registerAs), key);

            /* Register. */
            models.SetModel(registerAs, key, part0 ?? key, category);
        }

        /// <summary>
        /// Register a model in the parser.
        /// </summary>
        /// <typeparam name="TModel">Model type.</typeparam>
        /// <param name="key">Block key for the model.</param>
        /// <param name="name">Display name for the model.</param>
        /// <param name="category">Toolbox category to use.</param>
        public void AddModel<TModel>(string key, string name, string? category = null) where TModel : class, new()
            => _models.Add<TModel>(key, () =>
                {
                    /* Initialize the generic model generator. */
                    var (blockDefinition, toolboxEntry) = ModelBlock<TModel>.Initialize(key, name, category, _models, CreateScratchModel);

                    /* Register the block and toolbox definition in the parser. */
                    _parser.ModelDefinitions.Add(blockDefinition);
                    _parser.ToolboxEntries.Add(Tuple.Create(string.IsNullOrEmpty(category) ? "*" : category, toolboxEntry));

                    /* Add the related block to the parser. */
                    _parser.AddBlock<ModelBlock<TModel>>(key);

                    /* Register. */
                    models.SetModel<TModel>(key, name, category);
                });

        /// <summary>
        /// See if we can create a model on the fly.
        /// </summary>
        /// <param name="type">Type to create a model for.</param>
        /// <param name="key">Blockly key of the potentially created model.</param>
        /// <param name="name">Name of the parent model.</param>
        /// <param name="category">Toolbox category to use.</param>
        /// <returns>Set if a model has been created.</returns>
        private bool CreateScratchDictionary(Type type, string key, string name, string? category)
        {
            /* Key must be a known enumeration. */
            var keyType = type.GetGenericArguments()[0];

            if (!keyType.IsEnum || !_models.Contains(keyType)) return false;

            /* Create the model. */
            var addModelMethod = GetType().GetMethod("AddModel")!;
            var addModel = addModelMethod.MakeGenericMethod(type);

            addModel.Invoke(this, [key, $"{name} {key.Split("_").Last()}", category]);

            /* Did it. */
            return true;
        }

        /// <summary>
        /// See if we can create a model on the fly.
        /// </summary>
        /// <param name="type">Type to create a model for.</param>
        /// <param name="key">Blockly key of the potentially created model.</param>
        /// <param name="name">Name of the parent model.</param>
        /// <param name="category">Toolbox category to use.</param>
        /// <returns>Set if a model has been created.</returns>
        private bool CreateScratchList(Type type, string key, string name, string? category)
        {
            /* Create the model. */
            var addModelMethod = GetType().GetMethod("AddModel")!;
            var addModel = addModelMethod.MakeGenericMethod(type);

            addModel.Invoke(this, [key, $"{name} {key.Split("_").Last()}", category]);

            /* Did it. */
            return true;
        }

        /// <summary>
        /// See if we can create a model on the fly.
        /// </summary>
        /// <param name="type">Type to create a model for.</param>
        /// <param name="key">Blockly key of the potentially created model.</param>
        /// <param name="name">Name of the parent model.</param>
        /// <param name="category">Toolbox category to use.</param>
        /// <returns>Set if a model has been created.</returns>
        private bool CreateScratchModel(Type type, string key, string name, string? category)
        {
            if (!type.IsGenericType) return false;

            var generic = type.GetGenericTypeDefinition();

            /* See if the type is a dictionary. */
            if (generic == typeof(Dictionary<,>)) return CreateScratchDictionary(type, key, name, category);

            /* See if the type is a list. */
            if (generic == typeof(List<>)) return CreateScratchList(type, key, name, category);

            return false;
        }

        /// <summary>
        /// Register a enumeration in the parser.
        /// </summary>
        /// <typeparam name="T">Enumeration type.</typeparam>
        /// <param name="key">Block key for the enumeration.</param>
        /// <param name="name">Display name for the enumeration.</param>
        /// <param name="category">Toolbox category to put the enumeration in.</param>
        public void AddEnum<T>(string key, string name, string? category = null) where T : Enum
            => _models.Add<T>(key, () =>
            {
                /* Initialize the generic model generator. */
                var (blockDefinition, toolboxEntry) = EnumBlock<T>.Initialize(key, name);

                /* Register the block and toolbox definition in the parser. */
                _parser.ModelDefinitions.Add(blockDefinition);
                _parser.ToolboxEntries.Add(Tuple.Create(string.IsNullOrEmpty(category) ? "Enum" : category, toolboxEntry));

                /* Add the related block to the parser. */
                _parser.AddBlock<EnumBlock<T>>(key);

                /* Register. */
                models.SetEnum<T>(key, name, category);
            });
    }

    /// <summary>
    /// Configure a parser with all library and standard script blocks.
    /// </summary>
    /// <typeparam name="TParser">Type of the parser to use.</typeparam>
    /// <param name="parser">Parser to configure.</param>
    /// <param name="models">Remember all model blocks.</param>
    /// <param name="custom">Method to add custom definitions.</param>
    /// <returns>The configured parser.</returns>
    public static TParser AddCustomBlocks<TParser>(this TParser parser, IScriptModels models, Action<BlockBuilder<TParser>>? custom) where TParser : Parser<TParser>
    {
        /* Use our little helper. */
        var builder = BlockBuilder.Create(parser, models);

        /* Add library extensions: enumerations, any order. */
        builder.AddEnum<GroupResultType>("group_execution_result_type", "Result of the Exceution");

        /* Add library extensions: models, leaves first up to root. */
        builder.AddModel<GroupResult>("group_execution_result", "Result of a group execution");
        builder.AddModel<GroupStatusCommon>("group_execution_status", "Status of a group execution");

        /* Add library extensions: blocks, any order. */
        builder.AddBlock<CreateRunScriptParameter>();
        builder.AddBlock<Delay>();
        builder.AddBlock<ExecutionGroup>();
        builder.AddBlock<ExtractProperty>();
        builder.AddBlock<GetGroupStatus>();
        builder.AddBlock<HttpRequest>();
        builder.AddBlock<Now>();
        builder.AddBlock<ParseJson>();
        builder.AddBlock<ParseNumber>();
        builder.AddBlock<ReadFromModel>();
        builder.AddBlock<RequestUserInput>();
        builder.AddBlock<RunParallel>();
        builder.AddBlock<RunScript>();
        builder.AddBlock<SetProgress>();
        builder.AddBlock<Throw>();
        builder.AddBlock<TryCatchFinally>();
        builder.AddBlock<UpdateModelProperty>();

        /* Add custom definitions. */
        custom?.Invoke(builder);

        /* Create definitions. */
        builder.CreateDefinitions();

        /* And use the standard block definitions. */
        return parser.AddStandardBlocks();
    }
}
