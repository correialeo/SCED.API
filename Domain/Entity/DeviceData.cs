namespace SCED.API.Domain.Entity
{
    public class DeviceData
    {
        public long Id { get; set; }
        public long DeviceId { get; set; }
        public double Value { get; set; }
        public DateTime Timestamp { get; set; }

        //relationships
        public Device Device { get; set; }

        public DeviceData() { }

        public DeviceData(long deviceId, double value, DateTime timestamp)
        {
            DeviceId = deviceId;
            Value = value;
            Timestamp = timestamp;
        }
    }
}
