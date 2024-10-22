namespace BlocklyNet.Core.Model;

/// <summary>
/// Used by the return-if block to propagate the result
/// through all nested layers including try-catch-finally.
/// </summary>
internal class ReturnProcedureIfException(object? value) : Exception
{
    /// <summary>
    /// Return value of the procedure.
    /// </summary>
    public readonly object? Value = value;
}