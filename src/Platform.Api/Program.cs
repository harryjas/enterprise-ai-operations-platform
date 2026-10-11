using Microsoft.EntityFrameworkCore;
using Platform.Api.Models;
using Platform.Api.Data;
using Platform.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<PlatformDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var workItems = new List<WorkItem>();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));

app.MapGet("/api/work-items", async (PlatformDbContext dbContext) => await dbContext.WorkItems.AsNoTracking().ToListAsync());

app.MapPost("/api/work-items", async (CreateWorkItemRequest request, PlatformDbContext dbContext) =>
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
        TenantId = request.TenantId.Trim(),
        Status = request.Status?.Trim() ?? "Open"
    };

    dbContext.WorkItems.Add(workItem);
    await dbContext.SaveChangesAsync();

    return Results.Created($"/api/work-items/{workItem.Id}", workItem);
});

app.MapGet("/boom", () => { throw new InvalidOperationException("test failure"); });
app.Run();

public partial class Program { }