using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace BlocklyNet.Scripting.Engine;

public partial class ScriptEngine
{
    /// <summary>
    /// Current active user reply.
    /// </summary>
    private UserInputRequest? _inputRequest;

    /// <summary>
    /// Response trigger for user replies.
    /// </summary>
    private TaskCompletionSource<UserInputResponse>? _inputResponse;

    /// <inheritdoc/>
    public void SetUserInput(UserInputResponse response) => SetUserInput(response, true);

    private void SetUserInput(UserInputResponse response, bool mustLock)
    {
        TaskCompletionSource<UserInputResponse>? inputResponse;

        using (mustLock ? Lock.Wait() : null)
        {
            /* The script requesting the input must still be the active one. */
            if (_active == null || _active.JobId != response.JobId)
                throw new ArgumentException("jobId");

            /* See if there is anyone wating on the response and clear the pending request. */
            inputResponse = _inputResponse;

            if (inputResponse == null)
                return;

            _inputRequest = null;
            _inputResponse = null;
        }

        /* Set result outside of lock - just to reduce the minimal chance of a deadlock even further. */
        Logger.LogTrace("Script {JobId} is processing {Key}={Value}.", response.JobId, response.Key, response.Value);

        inputResponse.SetResult(response);
    }

    /// <inheritdoc/>
    public Task<T?> GetUserInput<T>(string key, string? type = null)
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

                /* Tell our clients that we would like to get some input. */
                var inputRequest = new UserInputRequest { JobId = _active.JobId, Key = key, ValueType = type };

                context?
                    .Send(ScriptEngineNotifyMethods.InputRequest, _inputRequest = inputRequest)
                    .ContinueWith(t => Logger.LogError("Failed to request user input for script {JobId}: {Exception}", inputRequest.JobId, t.Exception?.Message), TaskContinuationOptions.NotOnRanToCompletion);
            }

            /* Report a promise on the result. */
            Logger.LogTrace("Script {JobId} is requesting input for {Key}.", _active.JobId, key);

            return _inputResponse.Task.ContinueWith(t =>
            {
                /* object? will be serialized as a JsonElement. */
                var value = t.Result.Value;

                if (value is JsonElement json)
                    switch (json.ValueKind)
                    {
                        case JsonValueKind.String:
                            value = json.Deserialize<string>();
                            break;
                        case JsonValueKind.Number:
                            value = json.Deserialize<double>();
                            break;
                        case JsonValueKind.Null:
                            value = null;
                            break;
                        case JsonValueKind.True:
                            value = true;
                            break;
                        case JsonValueKind.False:
                            value = false;
                            break;
                    }

                return value == null ? default : (T?)value;
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }
}
