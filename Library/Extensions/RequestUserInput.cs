using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;

namespace BlocklyNet.Extensions;


/// <summary>
/// Request data from the user.
/// </summary>
[CustomBlock(
  "request_user_input",
  "Scripts",
  @"{
      ""message0"": ""AwaitUserInteraction %1 Key %2 Type %3 Required %4 Auto close after (s) %5 Exception on auto close %6"",
      ""args0"": [
          {
            ""type"": ""input_dummy""
          },
          {
            ""type"": ""input_value"",
            ""name"": ""KEY"",
            ""check"": ""String""
          },          
          {
            ""type"": ""input_value"",
            ""name"": ""TYPE""
          },
          {
            ""type"": ""input_value"",
            ""name"": ""REQUIRED"",
            ""check"": ""Boolean""
          },          
          {
            ""type"": ""input_value"",
            ""name"": ""DELAY"",
            ""check"": ""Number""
          },
          {
            ""type"": ""input_value"",
            ""name"": ""THROWMESSAGE"",
            ""check"": ""String""
          }
      ],
      ""output"": null,
      ""colour"": 230,
      ""tooltip"": ""Request interaction from the user"",
      ""helpUrl"": """"
  }",
  @"{
    ""inputs"": {
      ""KEY"": {
        ""shadow"": {
          ""type"": ""text"",
          ""fields"": {
            ""TEXT"": """"
          }
        }
      },
      ""TYPE"": {
        ""shadow"": {
          ""type"": ""text"",
          ""fields"": {
            ""TEXT"": """"
          }
        }
      }
    }
  }"
)]
public class RequestUserInput : Block
{
  /// <inheritdoc/>
  protected override async Task<object?> EvaluateAsync(Context context)
  {
    var delay = await Values.EvaluateAsync<double?>("DELAY", context, false);
    var key = await Values.EvaluateAsync<string>("KEY", context);
    var required = await Values.EvaluateAsync<bool?>("REQUIRED", context, false);
    var secs = delay.GetValueOrDefault(0);
    var type = await Values.EvaluateAsync<string>("TYPE", context, false);

    /* No delay necessary - just wait for the reply to be available. */
    if (secs <= 0) return await context.Engine.GetUserInputAsync<object>(key, type, required: required);

    var cancel = new CancellationTokenSource();
    var delayTask = Task.Delay(TimeSpan.FromSeconds(secs), cancel.Token);
    var inputTask = context.Engine.GetUserInputAsync<object>(key, type, delay, required);

    /* See which task terminates first. */
    if (inputTask == await Task.WhenAny(inputTask, delayTask))
    {
      /* Cancel timer. */
      await cancel.CancelAsync();

      /* Report result. */
      return await inputTask;
    }

    /* Simulate user input. */
    context.Engine.Engine.SetUserInput(null);

    /* May want to throw an exception. */
    var message = await Values.EvaluateAsync<string?>("THROWMESSAGE", context, false);

    if (message != null) throw new TimeoutException(message);

    return null;
  }
}