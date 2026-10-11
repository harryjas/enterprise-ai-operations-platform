using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Platform.Api.Data;

namespace Platform.Api.Tests;

public class ApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            var testConnection = new SqlConnectionStringBuilder(config.GetConnectionString("Default"))
            {
                InitialCatalog = "PlatformDb_Test"
            }.ConnectionString;

            services.RemoveAll<DbContextOptions<PlatformDbContext>>();
            services.AddDbContext<PlatformDbContext>(o => o.UseSqlServer(testConnection));
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
        db.Database.EnsureDeleted();
        db.Database.Migrate();
        return host;
    }
}