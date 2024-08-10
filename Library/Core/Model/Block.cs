namespace BlocklyNet.Core.Model;

/// <summary>
/// Describes a single block in the script. A block may produce
/// a result and may execute the next block in chain.
/// </summary>
public abstract class Block : IFragment
{
    /// <summary>
    /// Unique identifier of the block.
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// All fields (constant values) of the block.
    /// </summary>
    public Fields Fields { get; } = new();

    /// <summary>
    /// All (dynamic) values of the block - actually other
    /// blocks producing the value.
    /// </summary>
    public Values Values { get; } = new();

    /// <summary>
    /// All statements of the block - more or less variant
    /// of (dynamic) values.
    /// </summary>
    public Statements Statements { get; } = new();

    /// <summary>
    /// The type of the block.
    /// </summary>
    public string Type { get; set; } = null!;

    /// <summary>
    /// Next block in chain.
    /// </summary>
    public Block? Next { get; set; }

    /// <summary>
    /// All mutation (extra settings) of the block.
    /// </summary>
    public IList<Mutation> Mutations { get; } = [];

    /// <summary>
    /// All comments for the block.
    /// </summary>
    public IList<Comment> Comments { get; } = [];

    /// <inheritdoc/>
    public virtual async Task<object?> Evaluate(Context context)
    {
        /* Wait for debugger to allow execution. */
        await context.Engine.SingleStep(this);

        /* Always check for cancel before proceeding with the execution of the next block in chain. */
        context.Cancellation.ThrowIfCancellationRequested();

        /* Check for early stop. */
        if (context.Engine.CurrentScript?.Options?.ShouldStopNow?.Invoke() == true)
            throw new ScriptStoppedEarlyException();

        /* Run the next block if we are not forcefully exiting a loop. */
        if (Next != null && context.EscapeMode == EscapeMode.None)
            return await Next.Evaluate(context);

        return null;
    }
}
