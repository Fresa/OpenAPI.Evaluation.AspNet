using System.Net;
using System.Net.Http.Headers;
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
    public async Task When_getting_content()
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

    [Fact]
    public async Task When_posting_content()
    {
        Responses.Add("POST", JsonNode.Parse("""
            "123"
            """));
        using var client = CreateDefaultClient();
        var result = await client.PostAsync("http://localhost/v2/user", new StringContent("""
            {
                "first-name": "Foo",
                "last-name": "Bar"
            }
            """)
            {
                Headers = { ContentType = MediaTypeHeaderValue.Parse("application/json")}
            })
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