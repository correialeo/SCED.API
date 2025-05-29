using Microsoft.EntityFrameworkCore;
using SCED.API.Infrasctructure.Context;

namespace SCED.API.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = Settings.GetConnectionString();

        services.AddDbContext<DatabaseContext>(options =>
        {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), options =>
            {
                options.DisableBackslashEscaping();
                options.EnableRetryOnFailure();
            });
        });

        return services;
    }
}