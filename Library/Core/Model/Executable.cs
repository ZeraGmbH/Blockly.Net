namespace BlocklyNet.Core.Model;

/// <summary>
/// Represents something executable.
/// </summary>
public abstract class Executable : IFragment
{
    /// <summary>
    /// Unique name of the executable.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Block to calculate the value of the executable.
    /// </summary>
    public required Block? Block { get; set; }

    /// <summary>
    /// Use the block to evaluate the value.
    /// </summary>
    /// <param name="context">Current execution context.</param>
    /// <returns>The value.</returns>
    public Task<object?> EvaluateAsync(Context context)
    {
        /* See if script should be terminated. */
        context.Cancellation.ThrowIfCancellationRequested();

        /* Use the block to get the current value. */
        for (var block = Block; block != null; block = block.Next)
            if (block.Enabled)
                return block.EvaluateAsync(context);

        /* If there is no block the result will always be null. */
        return Task.FromResult<object?>(null);
    }
}

/// <summary>
/// Represents a single value.
/// </summary>
public class Value : Executable
{
}

/// <summary>
/// Represents a statement.
/// </summary>
public class Statement : Executable
{
}