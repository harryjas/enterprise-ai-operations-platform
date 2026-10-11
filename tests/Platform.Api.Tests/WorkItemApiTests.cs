using System.Net;
using System.Net.Http.Json;

namespace Platform.Api.Tests;

public class WorkItemApiTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _httpClient;

    public WorkItemApiTests(ApiFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    private record WorkItemResponse(string Title, string Status, string TenantId);

    [Fact]
    public async Task Post_then_Get_returns_the_created_work_item()
    {
        var created = await _httpClient.PostAsJsonAsync("/api/work-items",
            new { title = "Write integration test", status = "Todo", TenantId = "test-tenant" });

        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        var workItems = await _httpClient.GetFromJsonAsync<WorkItemResponse[]>("/api/work-items");
        Assert.NotNull(workItems);
        Assert.Contains(workItems, w => w.Title == "Write integration test" && w.Status == "Todo" && w.TenantId == "test-tenant");
    }
}