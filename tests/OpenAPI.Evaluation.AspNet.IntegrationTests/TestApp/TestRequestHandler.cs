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
    public Task InvokeAsync(HttpContext context)
    {
        if (!_responses.TryGetValue(context.Request.Method, out var response))
        {
            return context.Response.WriteAsync(
                $"Method '{context.Request.Method}' not registered");
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
            return context.Response.WriteAsJsonAsync(response);
        }
        context.Response.StatusCode = 500;
        return context.Response.WriteAsJsonAsync(responseEvaluation);
    }
}