using System.Collections.Immutable;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace OpenAPI.Evaluation.AspNet;

public static class OpenApiEvaluationExtensions
{
    public static OpenApiEvaluationResults EvaluateRequest(this Specification.OpenAPI openApiSpecification,
        HttpRequest request)
    {
        var requestUri = request.GetRequestUri();
        var method = request.Method;
        var body = request.Body.Read();
        var headers = request.GetHeaders();
        return openApiSpecification.EvaluateRequest(requestUri, method, headers, body);
    }

    public static async Task<OpenApiEvaluationResults> EvaluateRequestAsync(this Specification.OpenAPI openApiSpecification,
        HttpRequest request, CancellationToken cancellationToken = default)
    {
        var requestUri = request.GetRequestUri();
        var method = request.Method;
        var body = await request.Body.ReadAsync(cancellationToken)
            .ConfigureAwait(false);
        var headers = request.GetHeaders();
        return openApiSpecification.EvaluateRequest(requestUri, method, headers, body);
    }

    public static OpenApiEvaluationResults EvaluateResponse(this Specification.OpenAPI openApiSpecification,
        HttpResponse response)
    {
        var request = response.HttpContext.Request;
        var requestUri = request.GetRequestUri();
        var method = request.Method;
        var responseHeaders = response.GetHeaders();
        var responseCode = response.StatusCode;
        var responseContent = response.Body.Read();
        return openApiSpecification.EvaluateResponse(requestUri, method, responseCode, responseHeaders,
            responseContent);
    }

    internal static OpenApiEvaluationResults EvaluateResponse(this Specification.OpenAPI openApiSpecification,
        HttpContext context, int responseCode, IDictionary<string, IEnumerable<string>> responseHeaders, JsonNode? responseContent)
    {
        var request = context.Request;
        var requestUri = request.GetRequestUri();
        var method = request.Method;
        return openApiSpecification.EvaluateResponse(requestUri, method, responseCode, responseHeaders,
            responseContent);
    }

    public static async Task<OpenApiEvaluationResults> EvaluateResponseAsync(this Specification.OpenAPI openApiSpecification,
        HttpResponse response, CancellationToken cancellationToken = default)
    {
        var request = response.HttpContext.Request;
        var requestUri = request.GetRequestUri();
        var method = request.Method;
        var responseHeaders = response.GetHeaders();
        var responseCode = response.StatusCode;
        var responseContent = await response.Body.ReadAsync(cancellationToken)
            .ConfigureAwait(false);
        return openApiSpecification.EvaluateResponse(requestUri, method, responseCode, responseHeaders,
            responseContent);
    }
    
    private static IDictionary<string, IEnumerable<string>> GetHeaders(this HttpResponse response) =>
        response.Headers.AsEvaluable();
    private static IDictionary<string, IEnumerable<string>> GetHeaders(this HttpRequest request) =>
        request.Headers.AsEvaluable();
    private static IDictionary<string, IEnumerable<string>> AsEvaluable(this IHeaderDictionary headers) =>
        headers.ToImmutableDictionary(
            header => header.Key,
            header => (IEnumerable<string>)header.Value);

    private static Uri GetRequestUri(this HttpRequest request) =>
        new UriBuilder
        {
            Scheme = request.Scheme,
            Host = request.Host.Host,
            Port = request.Host.Port ?? -1,
            Path = request.Path.ToString(),
            Query = request.QueryString.ToString()
        }.Uri;

    private static JsonNode? Read(this Stream contentStream)
    {
        if (!contentStream.CanRead)
        {
            return null;
        }
        if (contentStream.ReadByte() == -1)
        {
            return null;
        }
        contentStream.Position = 0;
        var content = JsonNode.Parse(contentStream);
        contentStream.Position = 0;
        return content;
    }

    private static async Task<JsonNode?> ReadAsync(this Stream contentStream, CancellationToken cancellationToken)
    {
        if (!contentStream.CanRead)
        {
            return null;
        }
        var buffer = new byte[1];
        var result = await contentStream.ReadAsync(buffer, cancellationToken)
            .ConfigureAwait(false);
        if (result == 0)
        {
            return null;
        }
        contentStream.Position = 0;
        var content = JsonNode.Parse(contentStream);
        contentStream.Position = 0;
        return content;
    }
}