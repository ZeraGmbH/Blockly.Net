using Microsoft.Extensions.DependencyInjection;

namespace BlocklyNet.Core.Model;

/// <summary>
/// All values in a block.
/// </summary>
public class Values : Entities<Value>
{
    /// <summary>
    /// Retrieve the key to use for value identification.
    /// </summary>
    /// <param name="value">Some value.</param>
    /// <returns>The name of the value.</returns>
    protected override string GetKey(Value value) => value.Name;

    /// <summary>
    /// Evaluate a single value.
    /// </summary>
    /// <param name="name">Name of the value.</param>
    /// <param name="context">Execution context.</param>
    /// <param name="required">If set the indicated value must exist.</param>
    /// <returns>Current result of the value.</returns>
    /// <exception cref="ArgumentException">Value does not exist.</exception>
    public Task<object?> EvaluateAsync(string name, Context context, bool required = true)
    {
        /* See if the value is known */
        var value = TryGet(name);

        if (value == null)
        {
            /* The value is optional so just indicate a null result. */
            if (!required) return Task.FromResult((object?)null);

            /* Value must exist so throw an exception. */
            throw new ArgumentException($"value {name} not found");
        }

        /* Before executing see if the script should be cancelled. */
        context.Cancellation.ThrowIfCancellationRequested();

        /* Try to evaluate the value. */
        return value.EvaluateAsync(context);
    }

    /// <summary>
    /// Get a number - may be auto-converted from some other type.
    /// </summary>
    /// <param name="name">Name of the parameter.</param>
    /// <param name="context">Operation context.</param>
    /// <returns>Requested number.</returns>
    public async Task<double> EvaluateDoubleAsync(string name, Context context)
    {
        /* Retrieve raw value. */
        var raw = await EvaluateAsync(name, context) ?? throw new ArgumentException($"value {name} not found");

        /* Already a number. */
        if (raw is double num) return num;

        /* Check for converter. */
        var cvt = context.ServiceProvider.GetService<IDoubleExtractor>();

        if (cvt == null) return (double)raw;

        /* Run converter - report cast error on mismatch. */
        try
        {
            return cvt.GetNumber(raw);
        }
        catch (Exception)
        {
            return (double)raw;
        }
    }

    /// <summary>
    /// Evaluate a single value.
    /// </summary>
    /// <param name="name">Name of the value.</param>
    /// <param name="context">Execution context.</param>
    /// <param name="required">If set the indicated value must exist.</param>
    /// <typeparam name="T">Expected result type.</typeparam>
    /// <returns>Current result of the value.</returns>
    /// <exception cref="ArgumentException">Value does not exist.</exception>
    public async Task<T> EvaluateAsync<T>(string name, Context context, bool required = true)
    {
        /* Execute the script. */
        var result = await EvaluateAsync(name, context, required);

        /* Try to change type of result if possible. */
        return result == null ? default! : (T)result;
    }
}
