namespace BlocklyNet.Scripting.Engine;

/// <summary>
/// 
/// </summary>
public static class SemaphoreExtensions
{
    /// <summary>
    /// Helper to wait on a semaphore to synchronize data flow.
    /// </summary>
    /// <param name="semaphore">Semaphore to use.</param>
    private class Waiter(SemaphoreSlim semaphore) : IDisposable
    {
        /// <summary>
        /// Semaphore to wait on.
        /// </summary>
        private SemaphoreSlim? _semaphore = semaphore;

        /// <summary>
        /// Release semaphore once - although this should never happen
        /// disposing is safely protected against multi-use.
        /// </summary>
        public void Dispose() => Interlocked.Exchange(ref _semaphore, null)?.Release();
    }

    /// <summary>
    /// Extension method to wait on a sempahore.
    /// </summary>
    /// <param name="semaphore">Semaphore to wait on.</param>
    /// <returns>Waiting helper instance.</returns>
    public static async Task<IDisposable> CreateWaiterAsync(this SemaphoreSlim semaphore)
    {
        await semaphore.WaitAsync();

        return new Waiter(semaphore);
    }
}
