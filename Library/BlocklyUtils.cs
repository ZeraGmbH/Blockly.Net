using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace BlocklyNet;

/// <summary>
/// Some implementation helpers.
/// </summary>
public static class BlockyUtils
{
    /// <summary>
    /// Simulate task access to avoid warnings.
    /// </summary>
    /// <param name="task">Some task.</param>
    public static void Touch(this Task task) { }

    /// <summary>
    /// Take some data convert it to JSON and compress
    /// the result.
    /// </summary>
    /// <param name="data">Data to compress.</param>
    /// <typeparam name="T">Type of the data.</typeparam>
    /// <returns>Representation of the compressed string.</returns>
    public static string Compress<T>(T data)
    {
        // Convert object to JSON string representation.
        var asString = JsonSerializer.Serialize(data, JsonUtils.JsonSettings);

        // Compress string.
        using var memory = new MemoryStream();

        using (var zipper = new GZipStream(memory, CompressionLevel.Optimal))
        {
            zipper.Write(Encoding.UTF8.GetBytes(asString));
            zipper.Flush();
        }

        // Get text representation of compressed string.
        return "*" + Convert.ToBase64String(memory.ToArray());
    }

    /// <summary>
    /// Reconstruct object from compressed string.
    /// </summary>
    /// <param name="data">Compressed data as a string.</param>
    /// <typeparam name="T">Type of the data.</typeparam>
    /// <returns>Reconstructed data.</returns>
    public static T? Decompress<T>(string? data)
    {
        // Decompress string from compressed text representation if necessary.
        var asString = data ?? "null";

        if (asString.StartsWith('*'))
        {
            using var memory = new MemoryStream(Convert.FromBase64String(asString[1..]));
            using var zipper = new GZipStream(memory, CompressionMode.Decompress);
            using var reader = new StreamReader(zipper);

            asString = reader.ReadToEnd();
        }

        // Reconstruct object from JSON serialized string.
        return JsonSerializer.Deserialize<T>(asString, JsonUtils.JsonSettings);
    }
}