using System.Text.Json.Serialization;
using SCED.API.Domain.Enums;

namespace SCED.API.Domain.Entity
{
    public class Device
    {
        public long Id { get; set; }
        public DeviceType Type { get; set; }
        public DeviceStatus Status { get; set; } 
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        [JsonIgnore]
        public ICollection<DeviceData> DeviceData { get; set; } = new List<DeviceData>();

        public Device() { }
        public Device(DeviceType type, DeviceStatus status, double latitude, double longitude)
        {
            Type = type;
            Status = status;
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
