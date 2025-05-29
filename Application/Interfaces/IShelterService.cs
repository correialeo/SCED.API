using SCED.API.Domain.Entity;

namespace SCED.API.Application.Interfaces
{
    public interface IShelterService
    {
        Task<IEnumerable<Shelter>> GetNearbySheltersAsync(double latitude, double longitude, double radiusKm = 10.0);
        Task<IEnumerable<Shelter>> GetAvailableSheltersAsync();
        Task<Shelter> CreateShelterAsync(Shelter shelter);
        Task<IEnumerable<Shelter>> GetAllSheltersAsync();
        Task<Shelter> GetShelterByIdAsync(long id);
        Task<Shelter> UpdateShelterAsync(long id, Shelter updatedShelter);
        Task<bool> DeleteShelterAsync(long id);
        Task<bool> UpdateCapacityAsync(long id, int newCurrentOccupancy);
        Task<IEnumerable<Shelter>> GetSheltersByCapacityRangeAsync(int minCapacity, int maxCapacity);
    }
}