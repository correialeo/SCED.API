using SCED.API.Domain.Enums;

namespace SCED.API.Domain.Entity
{
    public class Resource
    {
        public long Id { get; set; }
        public ResourceType Type { get; set; }
        public int Quantity { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public ResourceStatus Status { get; set; }

        public Resource() { }

        public Resource(ResourceType type, int quantity, double latitude, double longitude, ResourceStatus status)
        {
            Type = type;
            Quantity = quantity;
            Latitude = latitude;
            Longitude = longitude;
            Status = status;
        }
    }
}
