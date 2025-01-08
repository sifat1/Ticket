using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging; // For logging

public static class HangfireConfig
{
    public static void AddHangfireConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            // Log a critical error if the connection string is missing
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole()); // Or your preferred logger
            var logger = loggerFactory.CreateLogger("HangfireConfig");
            logger.LogCritical("Hangfire connection string is missing. Check your configuration.");
            throw new InvalidOperationException("Hangfire connection string is missing."); // Or handle it differently
        }


        services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(connectionString, new PostgreSqlStorageOptions
            {
                SchemaName = "hangfire", // Customize schema if needed
                // Other storage options
            }));

        services.AddHangfireServer(options =>
        {
            options.ServerName = "YourServerName"; // Customize server name
            options.WorkerCount = Environment.ProcessorCount * 2; // Example worker count
        });

        // Add Global Filters (Example: Automatic Retry)
         GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 3 });

        // Add Recurring jobs (Example)
        //RecurringJob.AddOrUpdate("SomeRecurringJob", () => Console.WriteLine("Recurring Job Executed"), Cron.Minutely);
    }
}