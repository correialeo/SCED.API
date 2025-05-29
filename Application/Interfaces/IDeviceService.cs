using SCED.API.Domain.Entity;
using SCED.API.Domain.Enums;

namespace SCED.API.Application.Interfaces
{
    public interface IDeviceService
    {
        Task<IEnumerable<Device>> GetAllDevicesAsync();
        Task<Device> GetDeviceByIdAsync(long id);
        Task<Device> UpdateDeviceAsync(long id, Device updatedDevice);
        Task<Device> CreateDeviceAsync(Device device);
        Task<bool> DeleteDeviceAsync(long id);
        Task<IEnumerable<Device>> GetDevicesByTypeAsync(DeviceType type);
        Task<IEnumerable<Device>> GetDevicesByStatusAsync(DeviceStatus status);
        Task<IEnumerable<Device>> GetDevicesInRadiusAsync(double latitude, double longitude, double radiusKm);
        Task<Device?> GetDeviceWithDataAsync(long id);
    }
}