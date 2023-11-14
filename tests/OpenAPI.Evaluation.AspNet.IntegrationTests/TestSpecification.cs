using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using OpenAPI.Evaluation.AspNet.IntegrationTests.TestApp;
using OpenAPI.Evaluation.Client;
using OpenAPI.Evaluation.AspNet.IntegrationTests.OpenApi;
using Xunit.Abstractions;

namespace OpenAPI.Evaluation.AspNet.IntegrationTests;

public abstract class TestSpecification : WebApplicationFactory<TestProgram>
{
    protected ITestOutputHelper Output { get; }
    internal readonly Responses Responses = new();
    private static readonly Uri BaseUri = new("http://localhost");
    private const string OpenApiFileName = "test-api.yaml";
    private readonly Specification.OpenAPI _openApi = OpenApi.OpenApi.Load(OpenApiFileName, new Uri(BaseUri, OpenApiFileName));

    protected TestSpecification(ITestOutputHelper testOutputHelper)
    {
        Output = testOutputHelper;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseContentRoot(Directory.GetCurrentDirectory());
        builder.ConfigureServices(collection =>
        {
            collection
                .AddOpenApiEvaluation(_openApi)
                .AddEvaluationMiddlewareOptions(_ => new OpenApiEvaluationMiddlewareOptions());
            collection.AddSingleton(Responses);
        });
    }

    protected HttpClient CreateResponseValidatingClient() =>
        new(new OpenApiEvaluationResultWritingHandler(Output,
            new OpenApiEvaluationHandler(_openApi,
                new OpenApiEvaluationHandlerOptions
                {
                    EvaluateRequests = false,
                    EvaluateResponses = true
                },
                Server.CreateHandler())))
        {
            BaseAddress = BaseUri
        };
}
