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
      ""message0"": ""AwaitUserInteraction %1 %2 %3 %4 %5 %6 %7 %8 %9"",
      ""args0"": [
          {
            ""type"": ""input_dummy""
          },
          {
              ""type"": ""field_label_serializable"",
              ""name"": ""KEY"",
              ""text"": ""Key""
          },
          {
            ""type"": ""input_value"",
            ""name"": ""KEY"",
            ""check"": ""String""
          },
          {
              ""type"": ""field_label_serializable"",
              ""name"": ""TYPE"",
              ""text"": ""Type""
          },
          {
            ""type"": ""input_value"",
            ""name"": ""TYPE""
          },
          {
              ""type"": ""field_label_serializable"",
              ""name"": ""DELAY"",
              ""text"": ""Auto close after (s)""
          },
          {
            ""type"": ""input_value"",
            ""name"": ""DELAY"",
            ""check"": ""Number""
          },
          {
              ""type"": ""field_label_serializable"",
              ""name"": ""THROWMESSAGE"",
              ""text"": ""Exception on auto close""
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
  public override async Task<object?> Evaluate(Context context)
  {
    var key = await Values.Evaluate<string>("KEY", context);
    var type = await Values.Evaluate<string>("TYPE", context, false);
    var delay = await Values.Evaluate<double?>("DELAY", context, false);
    var secs = delay.GetValueOrDefault(0);

    /* No delay necessary - just wait for the reply to be available. */
    if (secs <= 0) return await context.Engine.GetUserInput<object>(key, type);

    var cancel = new CancellationTokenSource();
    var delayTask = Task.Delay(TimeSpan.FromSeconds(secs), cancel.Token);
    var inputTask = context.Engine.GetUserInput<object>(key, type, delay);

    /* User has terminated the task. */
    if (Task.WaitAny(inputTask, delayTask) == 0)
    {
      /* Cancel timer. */
      cancel.Cancel();

      /* Report result. */
      return inputTask.Result;
    }

    /* Simulate user input. */
    context.Engine.Engine.SetUserInput(null);

    /* May want to throw an exception. */
    var message = await Values.Evaluate<string?>("THROWMESSAGE", context, false);

    if (message != null) throw new TimeoutException(message);

    return null;
  }
}