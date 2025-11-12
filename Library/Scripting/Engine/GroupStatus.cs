using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// Result of a finished execution group.
/// </summary>
public class GroupStatusCommon
{
    /// <summary>
    /// Set to indicate that this is no a real group but a separate script.
    /// </summary>
    [NotNull, Required]
    public bool IsScript { get; set; }

    /// <summary>
    /// Unique identifier of the group.
    /// </summary>
    [NotNull, Required]
    public string Key { get; set; } = null!;

    /// <summary>
    /// Optional name of the group.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Optional details of the execution.
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Result of the execution.
    /// </summary>
    public string? Result { get; set; }

    /// <summary>
    /// Save result of a group in a serialized form.
    /// </summary>
    /// <param name="result">Result to remember.</param>
    /// <remarks>Without any external manipulation of JsonUtils.JsonSettings all
    /// type informations will be lost.</remarks>
    public void SetResult(GroupResult result)
    {
        // Convert object to JSON string representation.
        var asString = JsonSerializer.Serialize(result, JsonUtils.JsonSettings);

        // Compress string.
        using var memory = new MemoryStream();

        using (var zipper = new GZipStream(memory, CompressionLevel.Optimal))
        {
            zipper.Write(Encoding.UTF8.GetBytes(asString));
            zipper.Flush();
        }

        // Get text representation of compressed string.
        Result = "*" + Convert.ToBase64String(memory.ToArray());
    }

    /// <summary>
    /// Reconstruct the result from the JSON string.
    /// </summary>
    /// <returns>Recostructed result.</returns>
    public GroupResult? GetResult()
    {
        // Decompress string from compressed text representation if necessary.
        var asString = Result ?? "null";

        if (asString.StartsWith('*'))
        {
            using var memory = new MemoryStream(Convert.FromBase64String(asString[1..]));
            using var zipper = new GZipStream(memory, CompressionMode.Decompress);
            using var reader = new StreamReader(zipper);

            asString = reader.ReadToEnd();
        }

        // Reconstruct object from JSON serialized string.
        return JsonSerializer.Deserialize<GroupResult>(asString, JsonUtils.JsonSettings);
    }
}

/// <summary>
/// Result of a finished execution group.
/// </summary>
public class GroupStatus<TChild> : GroupStatusCommon where TChild : GroupStatus<TChild>
{
    /// <summary>
    /// All executions started before this group has been finished.
    /// </summary>
    [NotNull, Required]
    public List<TChild> Children { get; set; } = [];

    /// <summary>
    /// Some additional data to allow customization. Serialized to
    /// a string.
    /// </summary>
    public string? CustomizerBlob { get; set; }
}

/// <summary>
/// Status information.
/// </summary>
public class GroupStatus : GroupStatus<GroupStatus> { }