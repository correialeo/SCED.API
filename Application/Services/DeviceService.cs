using SCED.API.Application.Interfaces;
using SCED.API.Domain.Entity;
using SCED.API.Domain.Enums;
using SCED.API.Domain.Interfaces;

namespace SCED.API.Application.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeviceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<IEnumerable<Device>> GetAllDevicesAsync()
        {
            try
            {
                return await _unitOfWork.Devices.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao buscar todos os dispositivos.", ex);
            }
        }

        public async Task<Device?> GetDeviceByIdAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentException("O ID do dispositivo deve ser maior que zero.", nameof(id));

            try
            {
                return await _unitOfWork.Devices.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao buscar dispositivo com ID {id}.", ex);
            }
        }

        public async Task<Device?> GetDeviceWithDataAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentException("O ID do dispositivo deve ser maior que zero.", nameof(id));

            try
            {
                return await _unitOfWork.Devices.GetDeviceWithDataAsync(id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao buscar dispositivo com dados históricos (ID: {id}).", ex);
            }
        }

        public async Task<IEnumerable<Device>> GetDevicesByTypeAsync(DeviceType type)
        {
            if (!Enum.IsDefined(typeof(DeviceType), type))
                throw new ArgumentException("Tipo de dispositivo inválido.", nameof(type));

            try
            {
                return await _unitOfWork.Devices.GetDevicesByTypeAsync(type);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao buscar dispositivos do tipo {type}.", ex);
            }
        }
        public async Task<IEnumerable<Device>> GetDevicesByStatusAsync(DeviceStatus status)
        {
            if (!Enum.IsDefined(typeof(DeviceStatus), status))
                throw new ArgumentException("Status do dispositivo inválido.", nameof(status));

            try
            {
                return await _unitOfWork.Devices.GetDevicesByStatusAsync(status);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao buscar dispositivos com status {status}.", ex);
            }
        }

        public async Task<IEnumerable<Device>> GetDevicesInRadiusAsync(double latitude, double longitude, double radiusKm = 5.0)
        {
            if (latitude < -90 || latitude > 90)
                throw new ArgumentException("A latitude deve estar entre -90 e 90 graus.", nameof(latitude));
            if (longitude < -180 || longitude > 180)
                throw new ArgumentException("A longitude deve estar entre -180 e 180 graus.", nameof(longitude));
            if (radiusKm <= 0 || radiusKm > 1000)
                throw new ArgumentException("O raio deve estar entre 0 e 1000 km.", nameof(radiusKm));

            try
            {
                return await _unitOfWork.Devices.GetDevicesInRadiusAsync(latitude, longitude, radiusKm);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao buscar dispositivos na área especificada.", ex);
            }
        }

        public async Task<Device> CreateDeviceAsync(Device device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device), "O dispositivo não pode ser nulo.");

            ValidateDevice(device);

            try
            {
                await _unitOfWork.Devices.AddAsync(device);
                await _unitOfWork.SaveChangesAsync();
                return device;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao criar o dispositivo.", ex);
            }
        }

        public async Task<Device?> UpdateDeviceAsync(long id, Device updatedDevice)
        {
            if (id <= 0)
                throw new ArgumentException("O ID do dispositivo deve ser maior que zero.", nameof(id));
            if (updatedDevice == null)
                throw new ArgumentNullException(nameof(updatedDevice), "O dispositivo atualizado não pode ser nulo.");
            if (id != updatedDevice.Id)
                throw new ArgumentException("O ID fornecido não corresponde ao ID do dispositivo.", nameof(id));

            ValidateDevice(updatedDevice);

            try
            {
                Device existingDevice = await _unitOfWork.Devices.GetByIdAsync(id);
                if (existingDevice == null)
                    return null;

                existingDevice.Type = updatedDevice.Type;
                existingDevice.Status = updatedDevice.Status;
                existingDevice.Latitude = updatedDevice.Latitude;
                existingDevice.Longitude = updatedDevice.Longitude;

                await _unitOfWork.Devices.UpdateAsync(existingDevice);
                await _unitOfWork.SaveChangesAsync();
                return existingDevice;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao atualizar o dispositivo com ID {id}.", ex);
            }
        }

        public async Task<bool> DeleteDeviceAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentException("O ID do dispositivo deve ser maior que zero.", nameof(id));

            try
            {
                Device device = await _unitOfWork.Devices.GetByIdAsync(id);
                if (device == null)
                    return false;

                await _unitOfWork.Devices.DeleteAsync(device);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao deletar o dispositivo com ID {id}.", ex);
            }
        }

        private static void ValidateDevice(Device device)
        {
            if (!Enum.IsDefined(typeof(DeviceType), device.Type))
                throw new ArgumentException("Tipo de dispositivo inválido.", nameof(device.Type));
            if (!Enum.IsDefined(typeof(DeviceStatus), device.Status))
                throw new ArgumentException("Status do dispositivo inválido.", nameof(device.Status));
            if (device.Latitude < -90 || device.Latitude > 90)
                throw new ArgumentException("A latitude deve estar entre -90 e 90 graus.", nameof(device.Latitude));
            if (device.Longitude < -180 || device.Longitude > 180)
                throw new ArgumentException("A longitude deve estar entre -180 e 180 graus.", nameof(device.Longitude));
        }
    }
}