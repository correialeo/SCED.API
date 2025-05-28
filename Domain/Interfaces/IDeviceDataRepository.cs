using SCED.API.Domain.Entity;

namespace SCED.API.Domain.Interfaces
{
    public interface IDeviceDataRepository : IRepository<DeviceData>
    {
        Task<IEnumerable<DeviceData>> GetDataByDeviceIdAsync(long deviceId);
        Task<IEnumerable<DeviceData>> GetDataByDeviceIdAndPeriodAsync(long deviceId, DateTime from, DateTime to);
        Task<DeviceData?> GetLatestDataByDeviceIdAsync(long deviceId);
        Task<IEnumerable<DeviceData>> GetDataAboveValueAsync(long deviceId, double value);
    }
}