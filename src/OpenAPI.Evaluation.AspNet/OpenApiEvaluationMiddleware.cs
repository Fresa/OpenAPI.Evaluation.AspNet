using System.Net;
using Microsoft.AspNetCore.Http;

namespace OpenAPI.Evaluation.AspNet;

public class OpenApiEvaluationMiddleware
{
    private readonly Specification.OpenAPI _openApiDocument;
    private readonly OpenApiEvaluationMiddlewareOptions _evaluationOptions;
    private readonly RequestDelegate _next;

    public OpenApiEvaluationMiddleware(
        Specification.OpenAPI openApiDocument,
        OpenApiEvaluationMiddlewareOptions evaluationOptions, 
        RequestDelegate next)
    {
        _openApiDocument = openApiDocument;
        _evaluationOptions = evaluationOptions;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        OpenApiEvaluationResults evaluationResults;
        var cancellationToken = context.RequestAborted;

        if (_evaluationOptions.EvaluateRequests)
        {
            evaluationResults = await _openApiDocument.EvaluateRequestAsync(context.Request, cancellationToken)
                .ConfigureAwait(false);
            if (!evaluationResults.IsValid)
            {
                if (_evaluationOptions.ThrowOnRequestEvaluationFailure)
                {
                    throw CreateOpenApiEvaluationException(evaluationResults);
                }
                await WriteEvaluationResultsAsync(
                        context, 
                        evaluationResults, 
                        cancellationToken)
                    .ConfigureAwait(false);
                return;
            }
        }

        await _next.Invoke(context)
            .ConfigureAwait(false);

        if (!_evaluationOptions.EvaluateResponses)
            return;

        evaluationResults = await _openApiDocument.EvaluateResponseAsync(context.Response, cancellationToken)
            .ConfigureAwait(false);
        if (!evaluationResults.IsValid)
        {
            if (_evaluationOptions.ThrowOnResponseEvaluationFailure || context.Response.HasStarted)
                throw CreateOpenApiEvaluationException(evaluationResults);
            await WriteEvaluationResultsAsync(context, evaluationResults, cancellationToken)
                .ConfigureAwait(false);
        }
    }

    private static Task WriteEvaluationResultsAsync(
        HttpContext context,
        OpenApiEvaluationResults evaluationResults,
        CancellationToken cancellationToken)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        return context.Response.WriteAsJsonAsync(evaluationResults, cancellationToken: cancellationToken);
    }

    private static OpenApiEvaluationException CreateOpenApiEvaluationException(OpenApiEvaluationResults evaluationResults) =>
        new("Evaluation failed", evaluationResults);
}