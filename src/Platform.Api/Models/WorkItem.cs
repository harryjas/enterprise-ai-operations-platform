namespace Platform.Api.Models;

public sealed class WorkItem
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public required string Title { get; set; }

    public string Status { get; set; } = "Open";

    public required string TenantId { get; set; }
}