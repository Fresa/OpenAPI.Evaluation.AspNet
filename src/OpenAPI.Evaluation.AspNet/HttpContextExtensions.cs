using System.Collections.Immutable;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Nodes;

namespace OpenAPI.Evaluation.AspNet;

public static class HttpContextExtensions
{
    /// <summary>
    /// Gets the registered OpenApi specification.
    /// Make sure to call <see cref="ServiceCollectionExtensions.AddOpenApiEvaluation"/> before using this method.
    /// </summary>
    /// <param name="httpContext">The current http context</param>
    /// <returns></returns>
    public static Specification.OpenAPI GetOpenApiSpecification(this HttpContext httpContext) =>
        httpContext.RequestServices.GetRequiredService<Specification.OpenAPI>();

    public static Task<OpenApiEvaluationResults> EvaluateRequestAsync(this HttpContext httpContext, 
        CancellationToken cancellationToken = default)
    {
        var specification = httpContext.GetOpenApiSpecification();
        return specification.EvaluateRequestAsync(httpContext.Request, cancellationToken);
    }

    public static OpenApiEvaluationResults EvaluateRequest(this HttpContext httpContext)
    {
        var specification = httpContext.GetOpenApiSpecification();
        return specification.EvaluateRequest(httpContext.Request);
    }
    
    public static OpenApiEvaluationResults EvaluateResponse(this HttpContext httpContext, 
        int responseCode, 
        IHeaderDictionary? responseHeaders = null,
        JsonNode? responseContent = null)
    {
        var specification = httpContext.GetOpenApiSpecification();
        var headers = responseHeaders == null
            ? ImmutableDictionary<string, IEnumerable<string>>.Empty
            : responseHeaders.ToImmutableDictionary(pair => pair.Key, pair => (IEnumerable<string>)pair.Value);
        return specification.EvaluateResponse(httpContext, responseCode, headers, responseContent);
    }
}