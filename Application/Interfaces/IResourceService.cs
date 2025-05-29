using SCED.API.Domain.Entity;
using SCED.API.Domain.Enums;

namespace SCED.API.Application.Interfaces
{
    public interface IResourceService
    {
        Task<IEnumerable<Resource>> GetNearbyResourcesAsync(double latitude, double longitude, double radiusKm = 5.0);
        Task<Resource> CreateResourceAsync(Resource resource);
        Task<IEnumerable<Resource>> GetAllResourcesAsync();
        Task<Resource> GetResourceByIdAsync(long id);
        Task<Resource> UpdateResourceAsync(long id, Resource updatedResource);
        Task<bool> DeleteResourceAsync(long id);
        Task<IEnumerable<Resource>> GetResourcesByTypeAsync(ResourceType type);
        Task<IEnumerable<Resource>> GetResourcesByStatusAsync(ResourceStatus status);
        Task<IEnumerable<Resource>> GetAvailableResourcesAsync();
    }
}