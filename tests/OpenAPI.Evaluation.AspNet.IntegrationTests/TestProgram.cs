using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using OpenAPI.Evaluation.AspNet;
using OpenAPI.Evaluation.AspNet.IntegrationTests.TestApp;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.UseOpenApiEvaluation();
app.UseMiddleware<TestRequestHandler>();

app.Run();

[UsedImplicitly]
public partial class TestProgram { }