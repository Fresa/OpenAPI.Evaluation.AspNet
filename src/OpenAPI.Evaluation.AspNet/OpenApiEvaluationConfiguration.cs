using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace OpenAPI.Evaluation.AspNet;

public sealed class OpenApiEvaluationConfiguration
{
    private readonly IServiceCollection _serviceCollection;

    internal OpenApiEvaluationConfiguration(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    public OpenApiEvaluationConfiguration AddEvaluationMiddlewareOptions(
        Func<IServiceProvider, OpenApiEvaluationMiddlewareOptions> evaluationOptionsFactory)
    {
        _serviceCollection.Replace(ServiceDescriptor.Singleton(evaluationOptionsFactory));
        return this;
    }
}