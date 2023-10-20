using Microsoft.AspNetCore.Builder;

namespace OpenAPI.Evaluation.AspNet;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Inserts HTTP request and response evaluation based on OpenApi.
    /// Make sure to configure <see cref="ServiceCollectionExtensions.AddOpenApiEvaluation"/> before calling this method.
    /// </summary>
    /// <param name="builder">Application builder</param>
    /// <returns>Current application builder</returns>
    public static IApplicationBuilder UseOpenApiEvaluation(this IApplicationBuilder builder) => 
        builder.UseMiddleware<OpenApiEvaluationMiddleware>();
}