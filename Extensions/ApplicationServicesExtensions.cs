using SCED.API.Domain.Interfaces;
using SCED.API.Infrastructure.Repositories;
using SCED.API.Interfaces;
using SCED.API.Services;

namespace SCED.API.Extensions;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<DeviceDataService>();
        services.AddScoped<IResourceService, ResourceService>();
        services.AddScoped<IShelterService, ShelterService>();
        services.AddScoped<IDeviceService, DeviceService>();
        services.AddScoped<IAlertService, AlertService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAlertRepository, AlertRepository>();
        services.AddScoped<IDeviceRepository, DeviceRepository>();
        services.AddScoped<IShelterRepository, ShelterRepository>();
        services.AddScoped<IResourceRepository, ResourceRepository>();
        services.AddScoped<IDeviceDataRepository, DeviceDataRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}