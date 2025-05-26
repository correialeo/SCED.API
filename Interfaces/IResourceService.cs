using SCED.API.Domain.Entity;

namespace SCED.API.Interfaces
{
    public interface IResourceService
    {
        Task<IEnumerable<Resource>> GetNearbyResourcesAsync(double latitude, double longitude, double radiusKm = 5.0);
        Task<Resource> CreateResourceAsync(Resource resource);
        Task<IEnumerable<Resource>> GetAllResourcesAsync();
        Task<Resource> GetResourceByIdAsync(long id);
        Task<Resource> UpdateResourceAsync(Resource resource);
        Task<bool> DeleteResourceAsync(long id);
    }
}