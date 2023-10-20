# OpenAPI.Evaluation.AspNet
Adds ASP.NET support for [OpenAPI.Evaluation](https://github.com/Fresa/OpenAPI.Evaluation).

[![Continuous Delivery](https://github.com/Fresa/OpenAPI.Evaluation.AspNet/actions/workflows/cd.yml/badge.svg)](https://github.com/Fresa/OpenAPI.Evaluation.AspNet/actions/workflows/cd.yml)

## Installation
```Shell
dotnet add package AspNet.Evaluation.OpenAPI
```

https://www.nuget.org/packages/AspNet.Evaluation.OpenAPI/

## Getting Started
Load and register the OpenAPI specification, see [OpenAPI.Evaluation](https://github.com/Fresa/OpenAPI.Evaluation) for more details.
```dotnet
var builder = WebApplication.CreateBuilder(args);
using var openApiStream = File.OpenRead("path/to/openapi-specification.json");
var specification = OpenAPI.Evaluation.Specification.OpenAPI.Parse(JsonNode.Parse(openApiStream));
builder.Services.AddOpenApiEvaluation(specification);
var app = builder.Build();
// Register evaluation into the request pipeline
app.UseOpenApiEvaluation();
// Register route handlers
...
``` 
## Evaluating From a Route Handler
`HttpContext` has some extension methods which can be used to evaluate the current request and response.
```dotnet
app.Run(context =>
{
    var requestEvaluationResult = context.EvaluateRequest();
    ...
    var responseCode = ...
    IHeaderDictionary responseHeaders = ...
    var responseContent = ...
    var responseEvaluation = context.EvaluateResponse(responseCode, responseHeaders, responseContent);
    ...
    return Task.CompletedTask;
});
``` 
## Response Evaluation Caviat
Be aware that once response headers have been sent over the wire the response can no longer be changed. If you want to evaluate using the response make sure it's buffered. It's recommended to evaluate the response before setting it in the http context that way if evaluation fails it's possible to return a proper response or abort the request without leaking invalid information to the client.

# Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

# License
[MIT](LICENSE)