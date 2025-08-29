using Microsoft.EntityFrameworkCore;
using SCED.API.Infrasctructure.Context;

namespace SCED.API.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("sqlServer");
        services.AddDbContext<DatabaseContext>(options =>
        {
            options.UseSqlServer(connectionString, options =>
            {
                options.EnableRetryOnFailure();
            });
        });

        return services;
    }
}