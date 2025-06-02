using Microsoft.EntityFrameworkCore;
using SCED.API.Domain.Interfaces;
using SCED.API.Domain.Enums;
using SCED.API.Presentation.DTO;
using SCED.API.Domain.Entity;

namespace SCED.API.Application.Services
{
    public class StatisticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StatisticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<DashboardStatisticsDTO> GetDashboardStatisticsAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var from = fromDate ?? DateTime.UtcNow.AddMonths(-3);
                var to = toDate ?? DateTime.UtcNow;

                ValidateDateRange(from, to);

                var dashboard = new DashboardStatisticsDTO
                {
                    TotalAlerts = await GetTotalAlertsAsync(from, to),
                    ActiveDevices = await GetActiveDevicesCountAsync(),
                    TotalShelters = await GetTotalSheltersAsync(),
                    TotalResources = await GetTotalResourcesAsync(),
                    LocationStatistics = await GetLocationStatisticsAsync(from, to),
                    DeviceTypeStatistics = await GetDeviceTypeStatisticsAsync(from, to),
                    AlertTrends = await GetAlertTrendsAsync(from, to),
                    GeographicHotspots = await GetGeographicHotspotsAsync(from, to)
                };

                return dashboard;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao gerar estatísticas do dashboard", ex);
            }
        }

        public async Task<List<LocationStatisticsDTO>> GetLocationStatisticsAsync(DateTime fromDate, DateTime toDate, double? radiusKm = null, double? centerLat = null, double? centerLng = null)
        {
            try
            {
                ValidateDateRange(fromDate, toDate);
                ValidateLocationParameters(radiusKm, centerLat, centerLng);

                var alerts = await _unitOfWork.Context.Set<Alert>()
                    .Where(a => a.Timestamp >= fromDate && a.Timestamp <= toDate)
                    .Select(a => new { a.Latitude, a.Longitude, a.Type, a.Timestamp, a.Severity })
                    .ToListAsync();

                if (!alerts.Any())
                {
                    return new List<LocationStatisticsDTO>();
                }

                var locationGroups = alerts
                    .GroupBy(alert => new { 
                        Lat = Math.Round(alert.Latitude, 2), 
                        Lng = Math.Round(alert.Longitude, 2) 
                    })
                    .Select(g => new LocationStatisticsDTO
                    {
                        Latitude = g.Key.Lat,
                        Longitude = g.Key.Lng,
                        FloodIncidents = g.Count(a => a.Type == AlertType.Flood),
                        FireIncidents = g.Count(a => a.Type == AlertType.Fire),
                        EarthquakeIncidents = g.Count(a => a.Type == AlertType.Earthquake),
                        ExtremeTemperatureIncidents = g.Count(a => a.Type == AlertType.ExtremeHeat || a.Type == AlertType.ExtremeCold),
                        TotalIncidents = g.Count(),
                        LastIncident = g.Max(a => a.Timestamp),
                        RiskScore = CalculateRiskScore(g.Count(), g.Max(a => (int)a.Severity))
                    })
                    .ToList();

                if (radiusKm.HasValue && centerLat.HasValue && centerLng.HasValue)
                {
                    locationGroups = locationGroups.Where(r => 
                        CalculateDistance(r.Latitude, r.Longitude, centerLat.Value, centerLng.Value) <= radiusKm.Value
                    ).ToList();
                }

                return locationGroups.OrderByDescending(r => r.RiskScore).ToList();
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao obter estatísticas por localização", ex);
            }
        }

        public async Task<List<DeviceTypeStatisticsDTO>> GetDeviceTypeStatisticsAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                ValidateDateRange(fromDate, toDate);

                var deviceTypes = await _unitOfWork.Context.Set<Device>()
                    .Select(d => d.Type)
                    .Distinct()
                    .ToListAsync();

                if (!deviceTypes.Any())
                {
                    return new List<DeviceTypeStatisticsDTO>();
                }

                var result = new List<DeviceTypeStatisticsDTO>();

                foreach (var deviceType in deviceTypes)
                {
                    var deviceStats = await _unitOfWork.Context.Set<Device>()
                        .Where(d => d.Type == deviceType)
                        .GroupBy(d => d.Type)
                        .Select(g => new
                        {
                            DeviceType = g.Key,
                            TotalDevices = g.Count(),
                            ActiveDevices = g.Count(d => d.Status == DeviceStatus.Operational),
                            DeviceIds = g.Select(d => d.Id).ToList(),
                            Coordinates = g.Select(d => new { d.Latitude, d.Longitude }).ToList()
                        })
                        .FirstOrDefaultAsync();

                    if (deviceStats == null) continue;

                    var deviceDataStats = await _unitOfWork.Context.Set<DeviceData>()
                        .Where(dd => deviceStats.DeviceIds.Contains(dd.DeviceId) && 
                                   dd.Timestamp >= fromDate && 
                                   dd.Timestamp <= toDate)
                        .GroupBy(dd => 1)
                        .Select(g => new
                        {
                            Count = g.Count(),
                            Average = g.Average(x => x.Value),
                            Max = g.Max(x => x.Value),
                            Min = g.Min(x => x.Value),
                            LastReading = g.Max(x => x.Timestamp)
                        })
                        .FirstOrDefaultAsync();

                    int alertsCount = 0;
                    if (deviceStats.Coordinates.Any())
                    {
                        var alertsInPeriod = await _unitOfWork.Context.Set<Alert>()
                            .Where(a => a.Timestamp >= fromDate && a.Timestamp <= toDate)
                            .Select(a => new { a.Latitude, a.Longitude })
                            .ToListAsync();

                        alertsCount = alertsInPeriod.Count(alert => 
                            deviceStats.Coordinates.Any(coord => 
                                Math.Abs(alert.Latitude - coord.Latitude) < 0.01 && 
                                Math.Abs(alert.Longitude - coord.Longitude) < 0.01));
                    }

                    var stats = new DeviceTypeStatisticsDTO
                    {
                        DeviceType = deviceStats.DeviceType,
                        TotalDevices = deviceStats.TotalDevices,
                        ActiveDevices = deviceStats.ActiveDevices,
                        AlertsGenerated = alertsCount,
                        AverageValue = deviceDataStats?.Average ?? 0.0,
                        MaxValue = deviceDataStats?.Max ?? 0.0,
                        MinValue = deviceDataStats?.Min ?? 0.0,
                        LastReading = deviceDataStats?.LastReading ?? DateTime.MinValue
                    };

                    result.Add(stats);
                }

                return result;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao obter estatísticas por tipo de dispositivo", ex);
            }
        }

        public async Task<List<AlertTrendDTO>> GetAlertTrendsAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                ValidateDateRange(fromDate, toDate);

                var alertsData = await _unitOfWork.Context.Set<Alert>()
                    .Where(alert => alert.Timestamp >= fromDate && alert.Timestamp <= toDate)
                    .Select(alert => new { 
                        Date = alert.Timestamp.Date, 
                        alert.Type, 
                        alert.Severity 
                    })
                    .ToListAsync();

                if (!alertsData.Any())
                {
                    return new List<AlertTrendDTO>();
                }

                var trends = alertsData
                    .GroupBy(alert => new { alert.Date, alert.Type })
                    .Select(g => new AlertTrendDTO
                    {
                        Date = g.Key.Date,
                        AlertType = g.Key.Type,
                        Count = g.Count(),
                        Severity = (int)g.Average(a => (int)a.Severity)
                    })
                    .OrderBy(t => t.Date)
                    .ToList();

                return trends;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao obter tendências de alertas", ex);
            }
        }

        public async Task<List<GeographicHotspotDTO>> GetGeographicHotspotsAsync(DateTime fromDate, DateTime toDate, int topN = 10)
        {
            try
            {
                ValidateDateRange(fromDate, toDate);

                if (topN <= 0)
                    throw new ArgumentException("O número de hotspots deve ser maior que zero", nameof(topN));

                var alerts = await _unitOfWork.Context.Set<Alert>()
                    .Where(a => a.Timestamp >= fromDate && a.Timestamp <= toDate)
                    .Select(a => new { a.Latitude, a.Longitude, a.Type, a.Severity })
                    .ToListAsync();

                if (!alerts.Any())
                {
                    return new List<GeographicHotspotDTO>();
                }

                var hotspots = alerts
                    .GroupBy(alert => new { 
                        Lat = Math.Round(alert.Latitude, 3), 
                        Lng = Math.Round(alert.Longitude, 3) 
                    })
                    .Select(g => new GeographicHotspotDTO
                    {
                        Latitude = g.Key.Lat,
                        Longitude = g.Key.Lng,
                        AlertCount = g.Count(),
                        RiskLevel = CalculateRiskScore(g.Count(), g.Max(a => (int)a.Severity)),
                        PredominantAlertType = g.GroupBy(a => a.Type)
                                              .OrderByDescending(x => x.Count())
                                              .First().Key.ToString()
                    })
                    .OrderByDescending(h => h.AlertCount)
                    .Take(topN)
                    .ToList();

                return hotspots;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao obter hotspots geográficos", ex);
            }
        }

        public async Task UpdateRealTimeStatisticsAsync(long deviceId, double value)
        {
            try
            {
                if (deviceId <= 0)
                    throw new ArgumentException("ID do dispositivo deve ser maior que zero", nameof(deviceId));

                var device = await _unitOfWork.Devices.GetByIdAsync(deviceId);
                if (device == null)
                    throw new InvalidOperationException($"Dispositivo com ID {deviceId} não encontrado");

                await UpdateAggregatedStatistics(device, value);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao atualizar estatísticas em tempo real", ex);
            }
        }

        private async Task UpdateAggregatedStatistics(Device device, double value)
        {
            await Task.CompletedTask;
        }

        private static void ValidateDateRange(DateTime fromDate, DateTime toDate)
        {
            if (fromDate > toDate)
                throw new ArgumentException("A data inicial não pode ser maior que a data final");

            if (toDate > DateTime.UtcNow.AddDays(1))
                throw new ArgumentException("A data final não pode ser no futuro");

            if ((toDate - fromDate).TotalDays > 365)
                throw new ArgumentException("O período não pode ser maior que 365 dias");
        }

        private static void ValidateLocationParameters(double? radiusKm, double? centerLat, double? centerLng)
        {
            if (radiusKm.HasValue || centerLat.HasValue || centerLng.HasValue)
            {
                if (!radiusKm.HasValue || !centerLat.HasValue || !centerLng.HasValue)
                    throw new ArgumentException("Para filtrar por localização, todos os parâmetros (radiusKm, centerLat, centerLng) devem ser fornecidos");

                if (radiusKm <= 0)
                    throw new ArgumentException("O raio deve ser maior que zero", nameof(radiusKm));

                if (centerLat < -90 || centerLat > 90)
                    throw new ArgumentException("A latitude deve estar entre -90 e 90", nameof(centerLat));

                if (centerLng < -180 || centerLng > 180)
                    throw new ArgumentException("A longitude deve estar entre -180 e 180", nameof(centerLng));
            }
        }

        private static double CalculateRiskScore(int alertCount, int maxSeverity)
        {
            var baseScore = Math.Min(alertCount * 10, 70);
            var severityBonus = maxSeverity * 6;
            return Math.Min(baseScore + severityBonus, 100);
        }

        private static double CalculateDistance(double lat1, double lng1, double lat2, double lng2)
        {
            var R = 6371;
            var dLat = ToRadians(lat2 - lat1);
            var dLng = ToRadians(lng2 - lng1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double ToRadians(double degrees) => degrees * (Math.PI / 180);

        private async Task<int> GetTotalAlertsAsync(DateTime from, DateTime to)
        {
            return await _unitOfWork.Context.Set<Alert>()
                .CountAsync(a => a.Timestamp >= from && a.Timestamp <= to);
        }

        private async Task<int> GetActiveDevicesCountAsync()
        {
            return await _unitOfWork.Context.Set<Device>()
                .CountAsync(d => d.Status == DeviceStatus.Operational);
        }

        private async Task<int> GetTotalSheltersAsync()
        {
            return await _unitOfWork.Context.Set<Shelter>().CountAsync();
        }

        private async Task<int> GetTotalResourcesAsync()
        {
            return await _unitOfWork.Context.Set<Resource>().CountAsync();
        }
    }
}