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
    public Task<T?> GetUserInputAsync<T>(string key, string? type = null, double? delay = null, bool? required = null)
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
                    Required = required,
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

                /* Check for array of values - each with a dedicated type. */
                if (value is JsonElement array && array.ValueKind == JsonValueKind.Array && t.Result.ValueType?.StartsWith("[]") == true)
                {
                    /* Number of elements must match. */
                    var typeNames = t.Result.ValueType[2..].Split("⊕");

                    if (typeNames.Length == array.GetArrayLength())
                    {
                        /* Convert each value. */
                        var values = new List<object?>();

                        for (var i = 0; i < typeNames.Length; i++)
                            values.Add(UserInputValueFromJson(array[i], typeNames[i]));

                        /* Blind convert - hopefully caller knows what he is doing. */
                        return (T?)(object?)values.ToArray();
                    }
                }

                /* Fallback mode. */
                value = UserInputValueFromJson(value, t.Result.ValueType);

                return value == null ? default : (T?)value;
            }, CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Current);
        }
    }

    private object? UserInputValueFromJson(object? value, string? type)
    {
        /* Not JSON. */
        if (value is not JsonElement json) return value;

        /* Reconstruct to known model. */
        if (!string.IsNullOrEmpty(type) && ServiceProvider.GetRequiredService<IScriptModels>().Models.TryGetValue(type, out var model))
            return JsonSerializer.Deserialize(value?.ToString() ?? "null", model.Type, JsonUtils.JsonSettings);

        /* Just check for scalar. */
        return json.ToJsonScalar();
    }
}
