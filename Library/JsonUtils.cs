using System.Text.Json;
using System.Text.Json.Nodes;

namespace BlocklyNet;

/// <summary>
/// 
/// </summary>
public static class JsonUtils
{
    /// <summary>
    /// Configure serializer to generate camel casing.
    /// </summary>
    /// <remarks>
    /// Can be customized if consumer uses customized serializsation rules.
    /// </remarks>
#pragma warning disable CA2211 // Non-constant fields should not be visible
    public static JsonSerializerOptions JsonSettings = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
#pragma warning restore CA2211 // Non-constant fields should not be visible

    /// <summary>
    /// Extract a scalar value from a JsonElement.
    /// </summary>
    /// <param name="json">As parsed with the System.Text.Json library.</param>
    /// <returns>The value.</returns>
    public static object? ToJsonScalar(this JsonElement json)
        => json.ValueKind switch
        {
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            JsonValueKind.Number => json.Deserialize<double>(),
            JsonValueKind.String => json.Deserialize<string>(),
            JsonValueKind.True => true,
            _ => (object?)json,
        };

    /// <summary>
    /// Extract a scalar value from a JsonElement.
    /// </summary>
    /// <param name="json">As parsed with the System.Text.Json library.</param>
    /// <returns>The value.</returns>
    public static object? ToJsonScalar(this JsonNode json)
        => json.GetValueKind() switch
        {
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            JsonValueKind.Number => json.GetValue<double>(),
            JsonValueKind.String => json.GetValue<string>(),
            JsonValueKind.True => true,
            _ => (object?)json,
        };

    /// <summary>
    /// Deserialize a JSON element to an instance.
    /// </summary>
    /// <typeparam name="T">Type of the instance.</typeparam>
    /// <param name="node">Element to deserialize.</param>
    /// <returns>Instance - may be null if JSON element represents null.</returns>
    public static T? DefaultDeserialize<T>(this JsonNode node) => JsonSerializer.Deserialize<T>(node, JsonSettings);

    /// <summary>
    /// Deserialize a JSON element to an instance.
    /// </summary>
    /// <typeparam name="T">Type of the instance.</typeparam>
    /// <param name="element">Element to deserialize.</param>
    /// <returns>Instance - may be null if JSON element represents null.</returns>
    public static T? DefaultDeserialize<T>(this JsonElement element) => JsonSerializer.Deserialize<T>(element, JsonSettings);
}