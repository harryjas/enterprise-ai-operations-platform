using Platform.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var workItems = new List<WorkItem>();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));

app.MapGet("/api/work-items", () => Results.Ok(workItems));

app.MapPost("/api/work-items", (CreateWorkItemRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Title) ||
        string.IsNullOrWhiteSpace(request.TenantId))
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["title"] = ["Title is required."],
            ["tenantId"] = ["Tenant ID is required."]
        });
    }

    var workItem = new WorkItem
    {
        Title = request.Title.Trim(),
        TenantId = request.TenantId.Trim()
    };

    workItems.Add(workItem);

    return Results.Created($"/api/work-items/{workItem.Id}", workItem);
});

app.Run();