using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using FluentAssertions;
using Xunit.Abstractions;

namespace OpenAPI.Evaluation.AspNet.IntegrationTests;

public class OpenApi_Middleware_Tests : TestSpecification
{
    public OpenApi_Middleware_Tests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public async Task Test1()
    {
        Responses.Add("GET", JsonNode.Parse("""
            {
                "first-name": "Foo",
                "last-name": "Bar"
            }
            """));
        using var client = CreateDefaultClient();
        var result = await client.GetAsync("http://localhost/v1/user/1")
            .ConfigureAwait(false);
        var content = await result.Content.ReadFromJsonAsync<JsonNode>()
            .ConfigureAwait(false);
        content.Should().NotBeNull();
        Output.WriteLine(content!.ToJsonString(new JsonSerializerOptions
        {
            WriteIndented = true
        }));
        result.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}