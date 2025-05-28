using SCED.API.Domain.Entity;
using SCED.API.Domain.Enums;

namespace SCED.API.Domain.Interfaces
{
    public interface IDeviceRepository : IRepository<Device>
    {
        Task<IEnumerable<Device>> GetDevicesByTypeAsync(DeviceType type);
        Task<IEnumerable<Device>> GetDevicesByStatusAsync(DeviceStatus status);
        Task<IEnumerable<Device>> GetDevicesInRadiusAsync(double latitude, double longitude, double radiusKm);
        Task<Device?> GetDeviceWithDataAsync(long id);
    }
}