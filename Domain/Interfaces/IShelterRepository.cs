using SCED.API.Domain.Entity;

namespace SCED.API.Domain.Interfaces
{
    public interface IShelterRepository : IRepository<Shelter>
    {
        Task<IEnumerable<Shelter>> GetAvailableSheltersAsync();
        Task<IEnumerable<Shelter>> GetSheltersInRadiusAsync(double latitude, double longitude, double radiusKm);
        Task<IEnumerable<Shelter>> GetSheltersByCapacityRangeAsync(int minCapacity, int maxCapacity);
        Task<bool> UpdateOccupancyAsync(long id, int newOccupancy);
    }
}