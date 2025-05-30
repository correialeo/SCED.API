using Microsoft.EntityFrameworkCore;
using SCED.API.Domain.Entity;
using SCED.API.Domain.Enums;
using SCED.API.Domain.Interfaces;
using SCED.API.Infrasctructure.Context;

namespace SCED.API.Infrastructure.Repositories
{
    public class DeviceRepository : Repository<Device>, IDeviceRepository
    {
        public DeviceRepository(DatabaseContext context) : base(context) { }

        public async Task<IEnumerable<Device>> GetDevicesByTypeAsync(DeviceType type)
        {
            return await _dbSet.Where(d => d.Type == type).ToListAsync();
        }

        public async Task<IEnumerable<Device>> GetDevicesByStatusAsync(DeviceStatus status)
        {
            return await _dbSet.Where(d => d.Status == status).ToListAsync();
        }

        public async Task<IEnumerable<Device>> GetDevicesInRadiusAsync(double latitude, double longitude, double radiusKm)
        {
            var devices = await _dbSet.ToListAsync();
            return devices.Where(device => 
                CalculateDistance(latitude, longitude, device.Latitude, device.Longitude) <= radiusKm
            ).OrderBy(device => 
                CalculateDistance(latitude, longitude, device.Latitude, device.Longitude)
            );
        }

        public async Task<Device?> GetDeviceWithDataAsync(long id)
        {
            return await _dbSet
                .Include(d => d.DeviceData)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371;
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double degrees) => degrees * (Math.PI / 180);
    }
}