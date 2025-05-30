using Microsoft.EntityFrameworkCore;
using SCED.API.Domain.Entity;
using SCED.API.Domain.Enums;
using SCED.API.Domain.Interfaces;
using SCED.API.Infrasctructure.Context;

namespace SCED.API.Infrastructure.Repositories
{
    public class ResourceRepository : Repository<Resource>, IResourceRepository
    {
        public ResourceRepository(DatabaseContext context) : base(context) { }

        public async Task<IEnumerable<Resource>> GetResourcesByTypeAsync(ResourceType type)
        {
            return await _dbSet.Where(r => r.Type == type).ToListAsync();
        }

        public async Task<IEnumerable<Resource>> GetAvailableResourcesAsync()
        {
            return await _dbSet
                .Where(r => r.Status == ResourceStatus.Available && r.Quantity > 0)
                .ToListAsync();
        }

        public async Task<IEnumerable<Resource>> GetResourcesInRadiusAsync(double latitude, double longitude, double radiusKm)
        {
            var resources = await _dbSet.ToListAsync();
            return resources.Where(resource => 
                CalculateDistance(latitude, longitude, resource.Latitude, resource.Longitude) <= radiusKm
            ).OrderBy(resource => 
                CalculateDistance(latitude, longitude, resource.Latitude, resource.Longitude)
            );
        }

        public async Task<IEnumerable<Resource>> GetResourcesByStatusAsync(ResourceStatus status)
        {
            return await _dbSet.Where(r => r.Status == status).ToListAsync();
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371;
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double ToRadians(double degrees) => degrees * (Math.PI / 180);
    }
}