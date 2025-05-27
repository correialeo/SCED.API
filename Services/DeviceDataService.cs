using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SCED.API.Common;
using SCED.API.Domain.Entity;
using SCED.API.Domain.Enums;
using SCED.API.DTO;
using SCED.API.Infrasctructure.Context;

namespace SCED.API.Services
{
    public class DeviceDataService
    {
        private readonly DatabaseContext _context;
        
        public DeviceDataService(DatabaseContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ServiceResponse<DeviceData>> ReceiveDataAsync(DeviceDataDTO deviceDataDTO)
        {
            if (deviceDataDTO?.DeviceId <= 0)
            {
                return ServiceResponse<DeviceData>.CreateError("Invalid device ID");
            }

            DeviceData? deviceData = new DeviceData(deviceDataDTO.DeviceId, deviceDataDTO.Value);

            try
            {
                IExecutionStrategy executionStrategy = _context.Database.CreateExecutionStrategy();

                Device? device = null;
                Alert? alert = null;

                await executionStrategy.ExecuteAsync(async () =>
                {
                    using var transaction = await _context.Database.BeginTransactionAsync();

                    try
                    {
                        device = await _context.Devices
                            .FirstOrDefaultAsync(d => d.Id == deviceData.DeviceId);

                        if (device == null)
                        {
                            throw new Exception("Device not found");
                        }

                        _context.DeviceData.Add(deviceData);
                        await _context.SaveChangesAsync();

                        alert = await ProcessAlertLogicAsync(device, deviceData);

                        if (alert != null)
                        {
                            _context.Alerts.Add(alert);
                            await _context.SaveChangesAsync();
                        }

                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                });

                deviceData.Device = device;

                string message = alert != null 
                    ? "Device data received and alert generated" 
                    : "Device data received successfully";

                return ServiceResponse<DeviceData>.CreateSuccess(deviceData, message);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Device not found")
                {
                    return ServiceResponse<DeviceData>.CreateError("Device not found");
                }
                else
                {
                    return ServiceResponse<DeviceData>.CreateError($"Error receiving device data: {ex.Message}");
                }
            }
        }

        private async Task<Alert?> ProcessAlertLogicAsync(Device device, DeviceData deviceData)
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

        private Alert? ProcessTemperatureAlert(Device device, DeviceData deviceData)
        {
            if (deviceData.Value > 38)
            {
                return CreateAlert(
                    AlertType.ExtremeHeat, 3, device, deviceData,
                    $"Extreme temperature detected: {deviceData.Value}°C"
                );
            }
            
            if (deviceData.Value < 15)
            {
                return CreateAlert(
                    AlertType.ExtremeCold, 3, device, deviceData,
                    $"Extreme cold detected: {deviceData.Value}°C"
                );
            }
            
            return null;
        }

        private Alert? ProcessWaterLevelAlert(Device device, DeviceData deviceData)
        {
            return deviceData.Value > 100 
                ? CreateAlert(
                    AlertType.Flood, 5, device, deviceData,
                    $"High water level detected: {deviceData.Value}cm"
                )
                : null;
        }

        private Alert? ProcessVibrationAlert(Device device, DeviceData deviceData)
        {
            return deviceData.Value > 5 
                ? CreateAlert(
                    AlertType.Earthquake, 4, device, deviceData,
                    $"High vibration detected: {deviceData.Value}g"
                )
                : null;
        }

        private Alert? ProcessSmokeAlert(Device device, DeviceData deviceData)
        {
            return deviceData.Value > 200 
                ? CreateAlert(
                    AlertType.Fire, 4, device, deviceData,
                    $"High smoke level detected: {deviceData.Value}ppm"
                )
                : null;
        }

        private Alert CreateAlert(AlertType type, int severity, Device device, 
            DeviceData deviceData, string description)
        {
            return new Alert(
                type: type,
                severity: severity,
                latitude: device.Latitude,
                longitude: device.Longitude,
                timestamp: deviceData.Timestamp,
                description: description
            );
        }
    }
}