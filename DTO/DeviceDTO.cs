using SCED.API.Domain.Enums;

namespace SCED.API.DTO
{
    public class DeviceDTO
    {
        public DeviceType Type { get; set; }
        public DeviceStatus Status { get; set; } 
        public double Latitude { get; set; }
        public double Longitude { get; set; } 
    }
}