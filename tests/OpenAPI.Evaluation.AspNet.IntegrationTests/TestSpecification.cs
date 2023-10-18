using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using OpenAPI.Evaluation.AspNet.IntegrationTests.TestApp;
using Xunit.Abstractions;

namespace OpenAPI.Evaluation.AspNet.IntegrationTests;

public abstract class TestSpecification : WebApplicationFactory<TestProgram>
{
    protected ITestOutputHelper Output { get; }
    internal readonly Responses Responses = new();
    private readonly Uri _baseUri = new("http://localhost");

    protected TestSpecification(ITestOutputHelper testOutputHelper)
    {
        Output = testOutputHelper;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(collection =>
        {
            collection
                .AddOpenApiEvaluation(OpenApi.OpenApi.Load("test-api.yaml", _baseUri))
                .AddEvaluationMiddlewareOptions(_ => new OpenApiEvaluationMiddlewareOptions());
            collection.AddSingleton(Responses);
        });
    }
}