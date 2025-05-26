using SCED.API.Domain.Enums;

namespace SCED.API.Domain.Entity
{
    public class Alert
    {
        public long Id { get; set; }
        public AlertType Type { get; set; }
        public int Severity { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }
        public string Description { get; set; }

        public Alert() { }

        public Alert(AlertType type, int severity, double latitude, double longitude, DateTime timestamp, string description)
        {
            Type = type;
            Severity = severity;
            Latitude = latitude;
            Longitude = longitude;
            Timestamp = timestamp;
            Description = description;
        }
    }
}
