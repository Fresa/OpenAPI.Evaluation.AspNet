using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;

namespace OpenAPI.Evaluation.AspNet.IntegrationTests.TestApp;

internal sealed class TestRequestHandler
{
    private readonly Responses _responses;

    public TestRequestHandler(Responses responses, RequestDelegate _)
    {
        _responses = responses;
    }

    [UsedImplicitly]
    public async Task InvokeAsync(HttpContext context)
    {
        var requestCancellation = context.RequestAborted;
        
        context.Request.Body.Position = 0;
        var buffer = new byte[1];
        var requestBodyLengthRead = await context.Request.Body.ReadAsync(buffer, requestCancellation)
            .ConfigureAwait(false);
        if (context.Request.Body.Length > 0 && 
            requestBodyLengthRead == 0)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync("""
                "Expected request content"
                """, cancellationToken: requestCancellation);
            return;
        }

        if (!_responses.TryGetValue(context.Request.Method, out var response))
        {
            await context.Response.WriteAsync(
                $"Method '{context.Request.Method}' not registered", cancellationToken: requestCancellation);
            return;
        }

        IHeaderDictionary headers = new HeaderDictionary();
        headers.ContentType = "application/json";
        var responseEvaluation = context.EvaluateResponse(200, headers, response);
        if (responseEvaluation.IsValid)
        {
            foreach (var header in headers)
            {
                context.Response.Headers.Add(header);
            }
            await context.Response.WriteAsJsonAsync(response, cancellationToken: requestCancellation);
            return;
        }
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(responseEvaluation, cancellationToken: requestCancellation);
    }
}