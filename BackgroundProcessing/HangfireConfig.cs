using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
public static class HangfireConfig
{
    public static void AddHangfireConfig(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure Hangfire with PostgreSQL storage
        services.AddHangfire(x =>
            x.UsePostgreSqlStorage(configuration.GetConnectionString("DefaultConnection")));
        services.AddHangfireServer();
    }
}
