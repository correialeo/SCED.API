using Microsoft.EntityFrameworkCore;
using SCED.API.Domain.Entity;
using SCED.API.Infrasctructure.Context;
using SCED.API.Interfaces;

namespace SCED.API.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly DatabaseContext _context;

        public DeviceService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Device>> GetAllDevicesAsync()
        {
            return await _context.Devices.ToListAsync();
        }

        public async Task<Device> GetDeviceByIdAsync(long id)
        {
            return await _context.Devices.FindAsync(id);
        }

        public async Task<Device> UpdateDeviceAsync(long id, Device updatedDevice)
        {
            var device = await _context.Devices.FindAsync(id);
            if (device == null) return null;

            device.Type = updatedDevice.Type;
            device.Status = updatedDevice.Status;
            device.Latitude = updatedDevice.Latitude;
            device.Longitude = updatedDevice.Longitude;

            await _context.SaveChangesAsync();
            return device;
        }

        public async Task<Device> CreateDeviceAsync(Device device)
        {
            _context.Devices.Add(device);
            await _context.SaveChangesAsync();
            return device;
        }

        public async Task<bool> DeleteDeviceAsync(long id)
        {
            var device = await _context.Devices.FindAsync(id);
            if (device == null) return false;

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}