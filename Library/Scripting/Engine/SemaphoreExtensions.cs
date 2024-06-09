namespace BlocklyNet.Scripting.Engine;

public static class SemaphoreExtensions
{
    /// <summary>
    /// Helper to wait on a semaphore to synchronize data flow.
    /// </summary>
    private class Waiter : IDisposable
    {
        /// <summary>
        /// Semaphore to wait on.
        /// </summary>
        private Semaphore? _semaphore;

        /// <summary>
        /// Create wait in the semaphore.
        /// </summary>
        /// <param name="semaphore">Semaphore to use.</param>
        public Waiter(Semaphore semaphore) => (_semaphore = semaphore)?.WaitOne();

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
    public static IDisposable Wait(this Semaphore semaphore) => new Waiter(semaphore);
}
