using System.Diagnostics.CodeAnalysis;
using BlocklyNet.Scripting.Debugger;

namespace BlocklyNet.Core.Model;

/// <summary>
/// Describes a single block in the script. A block may produce
/// a result and may execute the next block in chain.
/// </summary>
public abstract class Block
{
    /// <summary>
    /// Unique identifier of the block.
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// Unset to exclude this block from execution.
    /// </summary>
    public bool Enabled { get; set; } = true;

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
    protected virtual async Task<object?> EvaluateAsync(Context context)
    {
        /* Wait for debugger to allow execution - the current block as finished its work. */
        await context.Engine.SingleStepAsync(this, context, ScriptDebuggerStopReason.Leave);

        /* Always check for cancel before proceeding with the execution of the next block in chain. */
        context.Cancellation.ThrowIfCancellationRequested();

        /* Check for early stop. */
        if (context.Engine.CurrentScript?.Options?.ShouldStopNow?.Invoke() == true)
            throw new ScriptStoppedEarlyException();

        /* Run the next block if we are not forcefully exiting a loop. */
        if (context.EscapeMode == EscapeMode.None)
            for (var next = Next; next != null; next = next.Next)
                if (next.Enabled)
                    return await next.EnterBlockAsync(context);

        return null;
    }

    /// <summary>
    /// Start a brand new block execution chain.
    /// </summary>
    /// <param name="context">Current operation context.</param>
    /// <returns>Result of the block if any.</returns>
    public async Task<object?> EnterBlockAsync(Context context)
    {
        /* Wait for debugger to allow execution - we enter a new chain of execution, e.g. calculating a value or control block. */
        await context.Engine.SingleStepAsync(this, context, ScriptDebuggerStopReason.Enter);

        /* Execute the block itself. */
        var result = await EvaluateAsync(context);

        /* Wait for debugger to allow execution - the current block as finished its work. */
        await context.Engine.SingleStepAsync(this, context, ScriptDebuggerStopReason.Finish);

        return result;
    }
}
