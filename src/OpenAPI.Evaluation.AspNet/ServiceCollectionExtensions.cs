using Microsoft.Extensions.DependencyInjection;

namespace OpenAPI.Evaluation.AspNet;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures OpenApi evaluation
    /// </summary>
    /// <param name="collection">Service collection</param>
    /// <param name="openApiSpecificationFactory">OpenApi specification instance factory</param>
    /// <returns>OpenApi evaluation configuration</returns>
    public static OpenApiEvaluationConfiguration AddOpenApiEvaluation(this IServiceCollection collection, 
        Func<IServiceProvider, Specification.OpenAPI> openApiSpecificationFactory)
    {
        collection.AddSingleton<OpenApiEvaluationMiddlewareOptions>();
        collection.AddSingleton(openApiSpecificationFactory);
        return new OpenApiEvaluationConfiguration(collection);
    }

    /// <summary>
    /// Configures OpenApi evaluation
    /// </summary>
    /// <param name="collection">Service collection</param>
    /// <param name="openApiSpecification">OpenApi specification instance</param>
    /// <returns>OpenApi evaluation configuration</returns>
    public static OpenApiEvaluationConfiguration AddOpenApiEvaluation(this IServiceCollection collection,
        Specification.OpenAPI openApiSpecification)
    {
        collection.AddSingleton<OpenApiEvaluationMiddlewareOptions>();
        collection.AddSingleton(openApiSpecification);
        return new OpenApiEvaluationConfiguration(collection);
    }
}