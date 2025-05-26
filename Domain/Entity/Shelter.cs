namespace SCED.API.Domain.Entity
{
    public class Shelter
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int Capacity { get; set; }
        public int CurrentOccupancy { get; set; } = 0;
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Shelter() { }

        public Shelter(string name, string address, int capacity, int currentOccupancy, double latitude, double longitude)
        {
            Name = name;
            Address = address;
            Capacity = capacity;
            CurrentOccupancy = currentOccupancy;
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
