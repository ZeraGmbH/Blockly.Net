namespace BlocklyNet.Core.Model;

/// <summary>
/// Comment in a block.
/// </summary>
/// <param name="comment">The comment text.</param>
public class Comment(string comment)
{
    /// <summary>
    /// The comment string.
    /// </summary>
    public string Value { get; set; } = comment;
}

