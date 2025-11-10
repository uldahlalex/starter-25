using api;
using dataccess;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

public static class DiExtensions
{
    public static void AddMyDbContext(this IServiceCollection services)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (environment != "Production")
        {
            var postgreSqlContainer = new PostgreSqlBuilder().Build();
            postgreSqlContainer.StartAsync().GetAwaiter().GetResult();
            var connectionString = postgreSqlContainer.GetConnectionString();
            services.AddDbContext<MyDbContext>((services, options) =>
            {
                options.UseNpgsql(connectionString);
            });
        }
        else
        {
            services.AddDbContext<MyDbContext>(
                (services, options) => { options.UseNpgsql(services.GetRequiredService<AppOptions>().Db); },
                ServiceLifetime.Transient);
        }
    }
    public static void InjectAppOptions(this IServiceCollection services)
    {
        services.AddSingleton<AppOptions>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var appOptions = new AppOptions();
            configuration.GetSection(nameof(AppOptions)).Bind(appOptions);
            return appOptions;
        });
    }
}