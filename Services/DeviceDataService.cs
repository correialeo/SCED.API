using Microsoft.EntityFrameworkCore;
using SCED.API.Domain.Entity;
using SCED.API.Domain.Enums;
using SCED.API.Domain.Interfaces;
using SCED.API.DTO;

namespace SCED.API.Services
{
    public class DeviceDataService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeviceDataService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<DeviceData> ReceiveDataAsync(DeviceDataDTO deviceDataDTO)
        {
            if (deviceDataDTO == null)
                throw new ArgumentNullException(nameof(deviceDataDTO), "Dados do dispositivo não podem ser nulos.");
            
            if (deviceDataDTO.DeviceId <= 0)
                throw new ArgumentException("O ID do dispositivo deve ser maior que zero.", nameof(deviceDataDTO.DeviceId));

            var deviceData = new DeviceData(deviceDataDTO.DeviceId, deviceDataDTO.Value);

            var strategy = _unitOfWork.Context.Database.CreateExecutionStrategy();
            
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _unitOfWork.Context.Database.BeginTransactionAsync();
                
                try
                {
                    var device = await _unitOfWork.Devices.GetByIdAsync(deviceData.DeviceId);
                    if (device == null)
                    {
                        throw new ArgumentException("Dispositivo não encontrado.", nameof(deviceData.DeviceId));
                    }

                    await _unitOfWork.DeviceData.AddAsync(deviceData);
                    await _unitOfWork.SaveChangesAsync();

                    var alert = await ProcessAlertLogicAsync(device, deviceData);
                    if (alert != null)
                    {
                        await _unitOfWork.Alerts.AddAsync(alert);
                        await _unitOfWork.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();
                    return deviceData;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        public async Task<IEnumerable<DeviceData>> GetDataByDeviceIdAsync(long deviceId)
        {
            if (deviceId <= 0)
                throw new ArgumentException("O ID do dispositivo deve ser maior que zero.", nameof(deviceId));

            try
            {
                return await _unitOfWork.DeviceData.GetDataByDeviceIdAsync(deviceId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao buscar dados do dispositivo {deviceId}.", ex);
            }
        }
        public async Task<IEnumerable<DeviceData>> GetDataByDeviceIdAndPeriodAsync(long deviceId, DateTime from, DateTime to)
        {
            if (deviceId <= 0)
                throw new ArgumentException("O ID do dispositivo deve ser maior que zero.", nameof(deviceId));

            if (from > to)
                throw new ArgumentException("A data inicial não pode ser posterior à data final.", nameof(from));

            if (to > DateTime.UtcNow)
                throw new ArgumentException("A data final não pode estar no futuro.", nameof(to));

            try
            {
                return await _unitOfWork.DeviceData.GetDataByDeviceIdAndPeriodAsync(deviceId, from, to);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao buscar dados do dispositivo {deviceId} no período especificado.", ex);
            }
        }

        public async Task<DeviceData?> GetLatestDataByDeviceIdAsync(long deviceId)
        {
            if (deviceId <= 0)
                throw new ArgumentException("O ID do dispositivo deve ser maior que zero.", nameof(deviceId));

            try
            {
                return await _unitOfWork.DeviceData.GetLatestDataByDeviceIdAsync(deviceId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao buscar dado mais recente do dispositivo {deviceId}.", ex);
            }
        }

        public async Task<IEnumerable<DeviceData>> GetDataAboveValueAsync(long deviceId, double value)
        {
            if (deviceId <= 0)
                throw new ArgumentException("O ID do dispositivo deve ser maior que zero.", nameof(deviceId));

            try
            {
                return await _unitOfWork.DeviceData.GetDataAboveValueAsync(deviceId, value);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao buscar dados acima de {value} para o dispositivo {deviceId}.", ex);
            }
        }

        private async Task<Alert?> ProcessAlertLogicAsync(Device device, DeviceData deviceData)
        {
            try
            {
                return device.Type switch
                {
                    DeviceType.TemperatureSensor => ProcessTemperatureAlert(device, deviceData),
                    DeviceType.WaterLevelSensor => ProcessWaterLevelAlert(device, deviceData),
                    DeviceType.VibrationSensor => ProcessVibrationAlert(device, deviceData),
                    DeviceType.SmokeSensor => ProcessSmokeAlert(device, deviceData),
                    _ => null
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private Alert? ProcessTemperatureAlert(Device device, DeviceData deviceData)
        {
            if (deviceData.Value > 38)
            {
                return CreateAlert(
                    AlertType.ExtremeHeat, 3, device, deviceData,
                    $"Temperatura extrema detectada: {deviceData.Value}°C"
                );
            }
            if (deviceData.Value < 15)
            {
                return CreateAlert(
                    AlertType.ExtremeCold, 3, device, deviceData,
                    $"Frio extremo detectado: {deviceData.Value}°C"
                );
            }
            return null;
        }

        private Alert? ProcessWaterLevelAlert(Device device, DeviceData deviceData)
        {
            return deviceData.Value > 100 
                ? CreateAlert(
                    AlertType.Flood, 5, device, deviceData,
                    $"Nível de água alto detectado: {deviceData.Value}cm"
                )
                : null;
        }

        private Alert? ProcessVibrationAlert(Device device, DeviceData deviceData)
        {
            return deviceData.Value > 5 
                ? CreateAlert(
                    AlertType.Earthquake, 4, device, deviceData,
                    $"Vibração alta detectada: {deviceData.Value}g"
                )
                : null;
        }

        private Alert? ProcessSmokeAlert(Device device, DeviceData deviceData)
        {
            return deviceData.Value > 200 
                ? CreateAlert(
                    AlertType.Fire, 4, device, deviceData,
                    $"Nível de fumaça alto detectado: {deviceData.Value}ppm"
                )
                : null;
        }

        private Alert CreateAlert(AlertType type, int severity, Device device, DeviceData deviceData, string description)
        {
            return new Alert
            {
                Type = type,
                Severity = severity,
                Latitude = device.Latitude,
                Longitude = device.Longitude,
                Timestamp = deviceData.Timestamp,
                Description = description
            };
        }
    }
}