using Hangfire;

namespace ShowTickets.BackgroundProcessing
{
    public static class HangfireConfigExtensions
    {
        public static void AddHangfireConfig(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure Hangfire to use SQL Server storage
            services.AddHangfire(config =>
                config.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));

            // Add Hangfire server
            services.AddHangfireServer();
        }
    }
}
