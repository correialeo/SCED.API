using System.Text.Json.Serialization;

namespace SCED.API.Domain.Entity
{
    public class DeviceData
    {
        public long Id { get; set; }
        public long DeviceId { get; set; }
        public double Value { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        //relationships
        [JsonIgnore]
        public Device Device { get; set; }

        public DeviceData() { }

        public DeviceData(long deviceId, double value)
        {
            DeviceId = deviceId;
            Value = value;
        }
    }
}
