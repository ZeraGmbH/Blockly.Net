using System.Text.Json;
using BlocklyNet.Extensions.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BlocklyNet.Scripting.Engine;

partial class ScriptEngine<TLogType>
{
    /// <summary>
    /// Current active user reply.
    /// </summary>
    private UserInputRequest? _inputRequest;

    /// <summary>
    /// Response trigger for user replies.
    /// </summary>
    private TaskCompletionSource<UserInputResponse>? _inputResponse;

    /// <summary>
    /// Exact time the input was requested.
    /// </summary>
    private DateTime _inputStarted = DateTime.MinValue;

    /// <summary>
    /// Optional planned seconds to auto-close the input request.
    /// </summary>
    private double? _inputDelay = null;

    /// <inheritdoc/>
    public void SetUserInput(UserInputResponse? response) => SetUserInput(response, true);

    private void SetUserInput(UserInputResponse? response, bool mustLock)
    {
        TaskCompletionSource<UserInputResponse>? inputResponse;

        using (mustLock ? Lock.Wait() : null)
        {
            /* The script requesting the input must still be the active one. */
            if (_active == null || (response != null && _active.JobId != response.JobId))
                throw new ArgumentException("jobId");

            /* See if there is anyone wating on the response. */
            inputResponse = _inputResponse;

            if (inputResponse == null)
                return;

            /* Copy from request. */
            response ??= new UserInputResponse
            {
                JobId = _active.JobId,
                Key = _inputRequest?.Key ?? string.Empty,
                ValueType = _inputRequest?.ValueType
            };

            /* Clear the pending request. */
            _inputRequest = null;
            _inputResponse = null;
        }

        /* Set result outside of lock - just to reduce the minimal chance of a deadlock even further. */
        Logger.LogTrace("Script {JobId} is processing {Key}={Value}.", response.JobId, response.Key, response.Value);

        inputResponse.SetResult(response);
    }

    /// <inheritdoc/>
    public Task<T?> GetUserInputAsync<T>(string key, string? type = null, double? delay = null)
    {
        using (Lock.Wait())
        {
            /* We have no active script. */
            if (_active == null)
                throw new InvalidOperationException("no active script.");

            /* If in the normal case there is no existing request just send the request to all clients. */
            if (_inputResponse == null)
            {
                /* Create a new response handler. */
                _inputResponse = new TaskCompletionSource<UserInputResponse>();
                _inputDelay = delay;
                _inputStarted = DateTime.UtcNow;

                /* Tell our clients that we would like to get some input. */
                var inputRequest = new UserInputRequest
                {
                    JobId = _active.JobId,
                    Key = key,
                    SecondsToAutoClose = _inputDelay,
                    StartedAt = _inputStarted,
                    ValueType = type,
                };

                _context?
                    .SendAsync(ScriptEngineNotifyMethods.InputRequest, _inputRequest = inputRequest)
                    .ContinueWith(
                        t => Logger.LogError("Failed to request user input for script {JobId}: {Exception}", inputRequest.JobId, t.Exception?.Message),
                        CancellationToken.None,
                        TaskContinuationOptions.NotOnRanToCompletion,
                        TaskScheduler.Current)
                    .Touch();
            }

            /* Report a promise on the result. */
            Logger.LogTrace("Script {JobId} is requesting input for {Key}.", _active.JobId, key);

            return _inputResponse.Task.ContinueWith(t =>
            {
                /* object? will be serialized as a JsonElement. */
                var value = t.Result.Value;

                if (value is JsonElement json)
                    if (!string.IsNullOrEmpty(t.Result.ValueType) && ServiceProvider.GetRequiredService<IScriptModels>().Models.TryGetValue(t.Result.ValueType, out var model))
                        /* Reconstruct to known model. */
                        value = JsonSerializer.Deserialize(value.ToString() ?? "null", model.Type, JsonUtils.JsonSettings);
                    else
                        /* Just check for scalar. */
                        value = json.ToJsonScalar();

                return value == null ? default : (T?)value;
            }, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Current);
        }
    }
}
