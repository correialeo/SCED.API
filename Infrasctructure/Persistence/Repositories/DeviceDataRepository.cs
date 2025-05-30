using Microsoft.EntityFrameworkCore;
using SCED.API.Domain.Entity;
using SCED.API.Domain.Interfaces;
using SCED.API.Infrasctructure.Context;

namespace SCED.API.Infrastructure.Repositories
{
    public class DeviceDataRepository : Repository<DeviceData>, IDeviceDataRepository
    {
        public DeviceDataRepository(DatabaseContext context) : base(context) { }

        public async Task<IEnumerable<DeviceData>> GetDataByDeviceIdAsync(long deviceId)
        {
            return await _dbSet
                .Where(dd => dd.DeviceId == deviceId)
                .OrderByDescending(dd => dd.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<DeviceData>> GetDataByDeviceIdAndPeriodAsync(long deviceId, DateTime from, DateTime to)
        {
            return await _dbSet
                .Where(dd => dd.DeviceId == deviceId && dd.Timestamp >= from && dd.Timestamp <= to)
                .OrderByDescending(dd => dd.Timestamp)
                .ToListAsync();
        }

        public async Task<DeviceData?> GetLatestDataByDeviceIdAsync(long deviceId)
        {
            return await _dbSet
                .Where(dd => dd.DeviceId == deviceId)
                .OrderByDescending(dd => dd.Timestamp)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<DeviceData>> GetDataAboveValueAsync(long deviceId, double value)
        {
            return await _dbSet
                .Where(dd => dd.DeviceId == deviceId && dd.Value > value)
                .OrderByDescending(dd => dd.Timestamp)
                .ToListAsync();
        }
    }
}