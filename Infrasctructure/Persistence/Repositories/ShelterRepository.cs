using Microsoft.EntityFrameworkCore;
using SCED.API.Domain.Entity;
using SCED.API.Domain.Interfaces;
using SCED.API.Infrasctructure.Context;

namespace SCED.API.Infrastructure.Repositories
{
    public class ShelterRepository : Repository<Shelter>, IShelterRepository
    {
        public ShelterRepository(DatabaseContext context) : base(context) { }

        public async Task<IEnumerable<Shelter>> GetAvailableSheltersAsync()
        {
            return await _dbSet
                .Where(s => s.CurrentOccupancy < s.Capacity)
                .OrderBy(s => s.CurrentOccupancy)
                .ToListAsync();
        }

        public async Task<IEnumerable<Shelter>> GetSheltersInRadiusAsync(double latitude, double longitude, double radiusKm)
        {
            var shelters = await _dbSet.ToListAsync();
            return shelters.Where(shelter => 
                CalculateDistance(latitude, longitude, shelter.Latitude, shelter.Longitude) <= radiusKm
            ).OrderBy(shelter => 
                CalculateDistance(latitude, longitude, shelter.Latitude, shelter.Longitude)
            );
        }

        public async Task<IEnumerable<Shelter>> GetSheltersByCapacityRangeAsync(int minCapacity, int maxCapacity)
        {
            return await _dbSet
                .Where(s => s.Capacity >= minCapacity && s.Capacity <= maxCapacity)
                .ToListAsync();
        }

        public async Task<bool> UpdateOccupancyAsync(long id, int newOccupancy)
        {
            var shelter = await _dbSet.FindAsync(id);
            if (shelter == null) return false;

            if (newOccupancy < 0 || newOccupancy > shelter.Capacity)
                throw new ArgumentException("Ocupação inválida");

            shelter.CurrentOccupancy = newOccupancy;
            return true;
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