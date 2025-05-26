using Microsoft.EntityFrameworkCore;
using SCED.API.Domain.Entity;
using SCED.API.Infrasctructure.Context;
using SCED.API.Interfaces;

namespace SCED.API.Services
{
    public class ResourceService : IResourceService
    {
        private readonly DatabaseContext _context;

        public ResourceService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Resource>> GetNearbyResourcesAsync(double latitude, double longitude, double radiusKm = 5.0)
        {
            IEnumerable<Resource> resources = await _context.Resources.ToListAsync();

            IOrderedEnumerable<Resource>? nearbyResources = resources.Where(resource =>
                CalculateDistance(latitude, longitude, resource.Latitude, resource.Longitude) <= radiusKm
            ).OrderBy(resource =>
                CalculateDistance(latitude, longitude, resource.Latitude, resource.Longitude)
            );

            return nearbyResources;
        }

        public async Task<Resource> CreateResourceAsync(Resource resource)
        {
            _context.Resources.Add(resource);
            await _context.SaveChangesAsync();
            return resource;
        }

        public async Task<IEnumerable<Resource>> GetAllResourcesAsync()
        {
            return await _context.Resources.ToListAsync();
        }

        public async Task<Resource> GetResourceByIdAsync(long id)
        {
            return await _context.Resources.FindAsync(id);
        }

        public async Task<Resource> UpdateResourceAsync(long id, Resource updatedResource)
        {
            Resource? resource = await _context.Resources.FindAsync(id);
            if (resource == null)
                return null;

            resource.Type = updatedResource.Type;
            resource.Quantity = updatedResource.Quantity;
            resource.Latitude = updatedResource.Latitude;
            resource.Longitude = updatedResource.Longitude;
            resource.Status = updatedResource.Status;

            await _context.SaveChangesAsync();
            return resource;
        }

        public async Task<bool> DeleteResourceAsync(long id)
        {
            var resource = await _context.Resources.FindAsync(id);
            if (resource == null)
                return false;

            _context.Resources.Remove(resource);
            await _context.SaveChangesAsync();
            return true;
        }

        // formula de haversine
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // raio da terra em km

            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = R * c;

            return distance;
        }

        private double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
    }
}