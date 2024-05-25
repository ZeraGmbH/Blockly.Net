using BlocklyNet.Core;
using BlocklyNet.Core.Blocks;
using BlocklyNet.Core.Model;
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
        private readonly Dictionary<Type, string> _models = [];

        /// <summary>
        /// The parser to configure.
        /// </summary>
        private readonly Parser<TParser> _parser = parser;

        /// <summary>
        /// Add a self describing block class to the parser.
        /// </summary>
        /// <typeparam name="TBlock">Type of the block.</typeparam>
        public void AddBlock<TBlock>() where TBlock : Block, new()
        {
            /* Aattribute indicator is required. */
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
        }

        /// <summary>
        /// Register a model in the parser.
        /// </summary>
        /// <typeparam name="T">Model type.</typeparam>
        /// <param name="key">Block key for the model.</param>
        /// <param name="name">Display name for the model.</param>
        public void AddModel<T>(string key, string name) where T : class, new()
        {
            /* Initialize the generic model generator. */
            var (blockDefinition, toolboxEntry) = ModelBlock<T>.Initialize(key, name, _models);

            /* Register the block and toolbox definition in the parser. */
            _parser.ModelDefinitions.Add(blockDefinition);
            _parser.ToolboxEntries.Add(Tuple.Create("*", toolboxEntry));

            /* Add the related block to the parser. */
            _parser.AddBlock<ModelBlock<T>>(key);

            /* Remember the model type - future models can now reference the property type. */
            _models[typeof(T)] = key;

            /* Register. */
            models.SetModel<T>(key, name);
        }

        /// <summary>
        /// Register a enumeration in the parser.
        /// </summary>
        /// <typeparam name="T">Enumeration type.</typeparam>
        /// <param name="key">Block key for the enumeration.</param>
        /// <param name="name">Display name for the enumeration.</param>
        public void AddEnum<T>(string key, string name) where T : Enum
        {
            /* Initialize the generic model generator. */
            var (blockDefinition, toolboxEntry) = EnumBlock<T>.Initialize(key, name);

            /* Register the block and toolbox definition in the parser. */
            _parser.ModelDefinitions.Add(blockDefinition);
            _parser.ToolboxEntries.Add(Tuple.Create("Enum", toolboxEntry));

            /* Add the related block to the parser. */
            _parser.AddBlock<EnumBlock<T>>(key);

            /* Remember the model type - future models can now reference the property type. */
            _models[typeof(T)] = key;

            /* Register. */
            models.SetEnum<T>(key, name);
        }
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

        /* Add library extensions: models, leaves first up to root. */

        /* Add library extensions: blocks, any order. */
        builder.AddBlock<CreateRunScriptParameter>();
        builder.AddBlock<Delay>();
        builder.AddBlock<ExtractProperty>();
        builder.AddBlock<HttpRequest>();
        builder.AddBlock<Now>();
        builder.AddBlock<ParseJson>();
        builder.AddBlock<ReadFromModel>();
        builder.AddBlock<RunParallel>();
        builder.AddBlock<RunScript>();
        builder.AddBlock<SetProgress>();
        builder.AddBlock<Throw>();
        builder.AddBlock<TryCatchFinally>();
        builder.AddBlock<UpdateModelProperty>();

        /* Add custom definitions. */
        custom?.Invoke(builder);

        /* And use the standard block definitions. */
        return parser.AddStandardBlocks();
    }
}
