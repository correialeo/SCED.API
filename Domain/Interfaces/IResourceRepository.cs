using SCED.API.Domain.Entity;
using SCED.API.Domain.Enums;

namespace SCED.API.Domain.Interfaces
{
    public interface IResourceRepository : IRepository<Resource>
    {
        Task<IEnumerable<Resource>> GetResourcesByTypeAsync(ResourceType type);
        Task<IEnumerable<Resource>> GetAvailableResourcesAsync();
        Task<IEnumerable<Resource>> GetResourcesInRadiusAsync(double latitude, double longitude, double radiusKm);
        Task<IEnumerable<Resource>> GetResourcesByStatusAsync(ResourceStatus status);
    }
}