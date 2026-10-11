namespace Platform.Api.Models;

public sealed record CreateWorkItemRequest(
    string Title,
    string TenantId,
    string? Status
);