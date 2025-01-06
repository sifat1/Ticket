using Hangfire;

public static class HangfireConfig
{
    public static void Configure(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(x => x.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));
        services.AddHangfireServer();
    }
}
