using BlocklyNet;
using BlocklyNet.Core.Blocks.Logic;
using BlocklyNet.Core.Blocks.Math;
using BlocklyNet.Core.Blocks.Text;
using BlocklyNet.Core.Blocks.Variables;
using BlocklyNet.Core.Model;
using BlocklyNet.Scripting.Definition;
using BlocklyNet.Scripting.Engine;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace BlocklyNetTests;

/// <summary>
/// Helpase base class for tests esp. providing a dependency injection core.
/// </summary>
public abstract class TestEnvironment
{
    /// <summary>
    /// Mocked store for script defnitions.
    /// </summary>
    private class Storage : IScriptDefinitionStorage
    {
        /// <summary>
        /// Minimum implementation for a script definition.
        /// </summary>
        private class Definition : IScriptDefinition
        {
            /// <inheritdoc/>
            public string Id { get; set; } = Guid.NewGuid().ToString();

            /// <inheritdoc/>
            public string Name { get; set; } = null!;

            /// <inheritdoc/>
            public string Code { get; set; } = null!;

            /// <inheritdoc/>
            public List<IScriptParameter> Parameters { get; set; } = [];

            /// <inheritdoc/>
            public string? ResultType { get; set; }
        }

        /* Our script definition database. */
        private readonly Dictionary<string, IScriptDefinition> _definitions = [];

        /* Find a script definition by its unique identifier. */
        public Task<IScriptDefinition?> Get(string id) => Task.FromResult(_definitions.TryGetValue(id, out var definition) ? definition : null);

        /* Query on script definitions. */
        public Task<IScriptDefinitionInfo> Find(string byName) => Task.FromResult(
            (IScriptDefinitionInfo)_definitions
                .Values
                .AsQueryable()
                .Where(s => s.Name.Equals(byName, StringComparison.CurrentCultureIgnoreCase))
                .Single());

        /// <summary>
        /// Add a single script definition to the mock.
        /// </summary>
        /// <param name="name">Display name of the script.</param>
        /// <param name="code">XML representation of the script's block tree.</param>
        /// <param name="args">Argument list of the script.</param>
        /// <returns>Unique identifier of the new script definition.</returns>
        public string Add(string name, string code, List<IScriptParameter> args)
        {
            /* Create a new instance, remember it and report the unique identifier. */
            var definition = new Definition { Name = name, Code = code, Parameters = args };

            _definitions.Add(definition.Id, definition);

            return definition.Id;
        }
    }

    /// <summary>
    /// Mock for the broadcast receiver.
    /// </summary>
    protected class Sink : IScriptEngineNotifySink
    {
        public Action<ScriptEngineNotifyMethods, object?>? OnEvent;

        /// <inheritdoc/>
        public Task Send(ScriptEngineNotifyMethods method, object? arg1)
        {
            OnEvent?.Invoke(method, arg1);

            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Helper class to simulate any type of data.
    /// </summary>
    protected class AnyValueBlock(object? value) : Block
    {
        /// <summary>
        /// Value to report.
        /// </summary>
        private readonly object? _value = value;

        /// <summary>
        /// Report the value.
        /// </summary>
        /// <param name="context">Execution context - will be ignored.</param>
        /// <returns>Our value.</returns>
        public override Task<object?> Evaluate(Context context) => Task.FromResult(_value);
    }

    /// <summary>
    /// In manual mode create a block reporting a constant number.
    /// </summary>
    /// <param name="value">Number as text.</param>
    /// <returns>New block.</returns>
    protected static Block CreateNumberBlock(string value)
        => new MathNumber { Fields = { new() { Name = "NUM", Value = value } } };

    /// <summary>
    /// In manual mode create a block reporting a constant string.
    /// </summary>
    /// <param name="value">String to report.</param>
    /// <returns>The new block.</returns>
    protected static Block CreateStringBlock(string value)
        => new TextBlock { Fields = { new() { Name = "TEXT", Value = value } } };

    /// <summary>
    /// In manual mode create a block reporting a constant boolean value.
    /// </summary>
    /// <param name="value">The string represenation of the boolean value,
    /// either TRUE or FALSE.</param>
    /// <returns>The new block.</returns>
    protected static Block CreateBooleanBlock(string value)
        => new LogicBoolean { Fields = { new() { Name = "BOOL", Value = value } } };

    /// <summary>
    /// In manual mode create a block reporting the current value of a variable,
    /// </summary>
    /// <param name="name">Name of the variable.</param>
    /// <returns>The new block.</returns>
    protected static Block CreateGetVariable(string name)
        => new VariablesGet { Fields = { new() { Name = "VAR", Value = name } } };

    /// <summary>
    /// In manual mode create a block setting the value of a variable.
    /// </summary>
    /// <param name="name">Name of the variable.</param>
    /// <param name="value">Block calculating the new value for the variable.</param>
    /// <param name="next">Optional next block to execute after the variable has been updated.</param>
    /// <returns>The new block.</returns>
    protected static Block CreateSetVariable(string name, Block value, Block? next = null)
        => new VariablesSet
        {
            Fields = { new() { Name = "VAR", Value = "dummy" } },
            Values = { new() { Name = "VALUE", Block = value } },
            Next = next
        };

    /* Dependency injection manager. */
    private ServiceProvider di = null!;

    /* Helper to mock cancel requests. */
    protected CancellationTokenSource Cancel = null!;

    /* Regular script engine. */
    protected IScriptEngine Engine { get; private set; } = null!;

    /* Mocked script site to execute single block trees without the full infrastructure. */
    protected Mock<IScriptSite> Site = null!;

    /// <summary>
    /// Override to initialize the dependency injection - empty in this base class.
    /// </summary>
    /// <param name="services">Configuration of the dependency injection.</param>
    protected virtual void OnSetup(IServiceCollection services)
    {
    }

    /// <summary>
    /// Override to initialize before the test starts.
    /// </summary>
    protected virtual void OnStartup()
    {
    }

    /// <summary>
    /// Register a script definition in the mocked store.
    /// </summary>
    /// <param name="name">Display name of the script.</param>
    /// <param name="code">Block tree in XML representation.</param>
    /// <param name="args">Argument list of the script.</param>
    /// <returns>Unique identifier of the new script definition.</returns>
    protected string AddScript(string name, string code, List<IScriptParameter>? args = null)
    {
        var storage = (Storage)GetService<IScriptDefinitionStorage>();

        return storage.Add(name, code, args ?? []);
    }

    /// <summary>
    /// Request a service from the dependency injection.
    /// </summary>
    /// <typeparam name="T">Type of the service.</typeparam>
    /// <returns>The requested service.</returns>
    protected T GetService<T>() where T : notnull => di.GetRequiredService<T>();

    /// <summary>
    /// Configure the dependency injection system before any test.
    /// </summary>
    [SetUp]
    public void SetupTestEnvironment()
    {
        /* Prepare an engine site mock for component tests, */
        Cancel = new CancellationTokenSource();

        Site = new Mock<IScriptSite>();

        Site.Setup(s => s.Cancellation).Returns(Cancel.Token);

        /* Setup dependency injection. */
        var services = new ServiceCollection();

        /* Enable logging. */
        services.AddLogging();

        /* Mocked script definition storage. */
        services.AddSingleton<IScriptDefinitionStorage, Storage>();

        /* Overall blockly configuration. */
        services.UseBlocklyNet();

        /* Allow per-test customizations. */
        OnSetup(services);

        di = services.BuildServiceProvider();

        /* Startup blockly runtime. */
        di.StartBlocklyNet();

        /* Request the real live engine */
        Engine = GetService<IScriptEngine>();

        /* Allow per test initializations. */
        OnStartup();
    }

    /// <summary>
    /// Cleanup after each test.
    /// </summary>
    [TearDown]
    public void TeardownTestEnvironment()
    {
        /* Shutdown cancel helper. */
        Cancel?.Dispose();
        Cancel = null!;

        /* Shutdown the dependency injection - this will especially dispose all singletons. */
        di?.Dispose();
        di = null!;
    }
}