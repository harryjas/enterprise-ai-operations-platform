using Microsoft.EntityFrameworkCore;
using Platform.Api.Models;

namespace Platform.Api.Data;

public class PlatformDbContext : DbContext
{
    public PlatformDbContext(DbContextOptions<PlatformDbContext> options) : base(options) { }

    public DbSet<WorkItem> WorkItems => Set<WorkItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkItem>(e =>
        {
            e.HasKey(w => w.Id);
            e.Property(w => w.Title).IsRequired().HasMaxLength(200);
            e.Property(w => w.Status).IsRequired().HasMaxLength(50);
            e.HasIndex(w => w.TenantId);
        });
    }
}