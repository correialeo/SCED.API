using System.Text.Json.Serialization;

namespace SCED.API.Extensions;

public static class ControllerExtensions
{
    public static IServiceCollection AddControllerServices(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        
        services.AddEndpointsApiExplorer();

        return services;
    }
}