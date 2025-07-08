using System.Text.Json;
using BlocklyNet.Extensions.Builder;
using NUnit.Framework;

namespace BlocklyNetTests.Customization;

[TestFixture]
public class ConfigurationTests : TestEnvironment
{
    /// <summary>
    /// Retrieve the overall configuration.
    /// </summary>
    [Test]
    public void Can_Get_Configuration()
    {
        var provider = GetService<IConfigurationService>();
        var anything = provider.Configuration;

        var self = Path.Combine(Path.GetDirectoryName(typeof(ConfigurationTests).Assembly.Location)!, "..", "..", "..", "app", "config.json");

        File.WriteAllText(self, JsonSerializer.Serialize(anything, new JsonSerializerOptions { WriteIndented = true }));
    }
}
