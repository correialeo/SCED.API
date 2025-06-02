using SCED.API.Domain.Enums;

namespace SCED.API.Presentation.DTO
{
    public class LocationStatisticsDTO
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public int FloodIncidents { get; set; }
        public int FireIncidents { get; set; }
        public int EarthquakeIncidents { get; set; }
        public int ExtremeTemperatureIncidents { get; set; }
        public int TotalIncidents { get; set; }
        public DateTime LastIncident { get; set; }
        public double RiskScore { get; set; } // 0-100
    }

    public class DeviceTypeStatisticsDTO
    {
        public DeviceType DeviceType { get; set; }
        public int TotalDevices { get; set; }
        public int ActiveDevices { get; set; }
        public int AlertsGenerated { get; set; }
        public double AverageValue { get; set; }
        public double MaxValue { get; set; }
        public double MinValue { get; set; }
        public DateTime LastReading { get; set; }
    }

    public class DashboardStatisticsDTO
    {
        public int TotalAlerts { get; set; }
        public int ActiveDevices { get; set; }
        public int TotalShelters { get; set; }
        public int TotalResources { get; set; }
        public List<LocationStatisticsDTO> LocationStatistics { get; set; } = new();
        public List<DeviceTypeStatisticsDTO> DeviceTypeStatistics { get; set; } = new();
        public List<AlertTrendDTO> AlertTrends { get; set; } = new();
        public List<GeographicHotspotDTO> GeographicHotspots { get; set; } = new();
    }

    public class AlertTrendDTO
    {
        public DateTime Date { get; set; }
        public AlertType AlertType { get; set; }
        public int Count { get; set; }
        public int Severity { get; set; }
    }

    public class GeographicHotspotDTO
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int AlertCount { get; set; }
        public double RiskLevel { get; set; }
        public string PredominantAlertType { get; set; } = string.Empty;
    }
}