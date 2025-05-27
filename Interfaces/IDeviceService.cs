using SCED.API.Domain.Entity;

namespace SCED.API.Interfaces
{
    public interface IDeviceService
    {
        Task<IEnumerable<Device>> GetAllDevicesAsync();
        Task<Device> GetDeviceByIdAsync(long id);
        Task<Device> UpdateDeviceAsync(long id, Device updatedDevice);
        Task<Device> CreateDeviceAsync(Device device);
        Task<bool> DeleteDeviceAsync(long id);
    }
}