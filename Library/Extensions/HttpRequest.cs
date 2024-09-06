using System.Net;
using System.Net.Http.Headers;
using System.Text;
using BlocklyNet.Core.Model;
using BlocklyNet.Extensions.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BlocklyNet.Extensions;

/// <summary>
/// Execute some HTTP request and report the result.
/// </summary>
[CustomBlock(
  "http_request",
  "",
  @"{
    ""message0"": ""HTTP %1 %2 %3 %4 %5 %6 %7 %8 %9 %10 %11"",
    ""args0"": [
      {
        ""type"": ""field_dropdown"",
        ""name"": ""METHOD"",
        ""options"": [
          [""GET"", ""GET""],
          [""POST"", ""POST""],
          [""PUT"", ""PUT""],
          [""DELETE"", ""DELETE""]
        ]
      },
      {
        ""type"": ""input_dummy""
      },
      {
          ""type"": ""field_label_serializable"",
          ""name"": ""URI"",
          ""text"": ""Url""
      },
      {
        ""type"": ""input_value"",
        ""name"": ""URI"",
        ""check"": ""String""
      },
      {
          ""type"": ""field_label_serializable"",
          ""name"": ""BODY"",
          ""text"": ""Payload""
      },
      {
        ""type"": ""input_value"",
        ""name"": ""BODY"",
        ""check"": ""String""
      },
      {
        ""type"": ""field_dropdown"",
        ""name"": ""AUTHMETHOD"",
        ""options"": [
          [""No Authentication"", """"],
          [""Bearer"", ""Bearer""],
          [""Basic"", ""Basic""]
        ]
      },
      {
          ""type"": ""field_label_serializable"",
          ""name"": ""AUTHTOKEN"",
          ""text"": ""Token""
      },
      {
        ""type"": ""input_value"",
        ""name"": ""AUTHTOKEN"",
        ""check"": ""String""
      },
      {
          ""type"": ""field_label_serializable"",
          ""name"": ""AUTHHEADER"",
          ""text"": ""HTTP-Header""
      },
      {
        ""type"": ""input_value"",
        ""name"": ""AUTHHEADER"",
        ""check"": ""String""
      }
    ],
    ""inputsInline"": false,
    ""output"": null,
    ""colour"": ""#107159"",
    ""tooltip"": ""HTTP Request""
  }",
  @"{
    ""inputs"": {
      ""URI"": {
        ""shadow"": {
          ""type"": ""text"",
          ""fields"": {
            ""TEXT"": """"
          }
        }
      },
      ""AUTHHEADER"": {
        ""shadow"": {
          ""type"": ""text"",
          ""fields"": {
            ""TEXT"": ""Authorization""
          }
        }
      }
    }
  }"
)]
public class HttpRequest : Block
{
  /// <inheritdoc/>
  public override async Task<object?> EvaluateAsync(Context context)
  {
    /* Get the endpoint. */
    var uri = await Values.EvaluateAsync<string>("URI", context);

    /* The body is optional and if given is expected to be in JSON string representation, */
    var body = await Values.EvaluateAsync<string>("BODY", context, false);
    var payload = body == null ? null : new StringContent(body, Encoding.UTF8, "application/json");

    /* Get a client to process the request. */
    using var client = context
      .ServiceProvider
      .GetRequiredService<IHttpClientFactory>()
      .CreateClient();

    /* Needs autherntication. */
    var auth = Fields["AUTHMETHOD"];

    if (!string.IsNullOrEmpty(auth))
    {
      var header = await Values.EvaluateAsync<string?>("AUTHHEADER", context, false);

      if (string.IsNullOrEmpty(header) || header == "Authorization")
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(auth, await Values.EvaluateAsync<string>("AUTHTOKEN", context));
      else
        client.DefaultRequestHeaders.Add(header, $"{auth} {await Values.EvaluateAsync<string>("AUTHTOKEN", context)}");
    }

    /* Generate the request from the configuration of the block. */
    Task<HttpResponseMessage> request = null!;

    switch (Fields["METHOD"])
    {
      case "GET":
        request = client.GetAsync(uri, context.Cancellation);
        break;
      case "DELETE":
        request = client.DeleteAsync(uri, context.Cancellation);
        break;
      case "POST":
        request = client.PostAsync(uri, payload, context.Cancellation);
        break;
      case "PUT":
        request = client.PutAsync(uri, payload, context.Cancellation);
        break;
    }

    /* Execute the request and wait for the response. */
    var response = await request;

    context.Cancellation.ThrowIfCancellationRequested();

    /* In case of any error just report the status code. */
    if (response.StatusCode != HttpStatusCode.OK)
      return (int)response.StatusCode;

    /* Report the resulting body as a string - further script steps may be necessary e.g for JSON parsing. */
    return await response.Content.ReadAsStringAsync();
  }
}
