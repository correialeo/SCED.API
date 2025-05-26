using Microsoft.EntityFrameworkCore;
using SCED.API.Domain.Entity;
using SCED.API.Infrasctructure.Context;
using SCED.API.Interfaces;

namespace SCED.API.Services
{

    public class ShelterService : IShelterService
    {
        private readonly DatabaseContext _context;

        public ShelterService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Shelter>> GetNearbySheltersAsync(double latitude, double longitude, double radiusKm = 10.0)
        {
            var shelters = await _context.Shelters.ToListAsync();
            
            var nearbyShelters = shelters.Where(shelter => 
                CalculateDistance(latitude, longitude, shelter.Latitude, shelter.Longitude) <= radiusKm
            ).OrderBy(shelter => 
                CalculateDistance(latitude, longitude, shelter.Latitude, shelter.Longitude)
            );

            return nearbyShelters;
        }

        public async Task<IEnumerable<Shelter>> GetAvailableSheltersAsync()
        {
            return await _context.Shelters
                .Where(s => s.CurrentOccupancy < s.Capacity)
                .OrderBy(s => s.CurrentOccupancy)
                .ToListAsync();
        }

        public async Task<Shelter> CreateShelterAsync(Shelter shelter)
        {
            if (shelter.Capacity <= 0)
                throw new ArgumentException("Capacidade deve ser maior que zero");

            if (shelter.CurrentOccupancy < 0)
                shelter.CurrentOccupancy = 0;

            if (shelter.CurrentOccupancy > shelter.Capacity)
                throw new ArgumentException("Ocupação atual não pode ser maior que a capacidade");
            
            _context.Shelters.Add(shelter);
            await _context.SaveChangesAsync();
            return shelter;
        }

        public async Task<IEnumerable<Shelter>> GetAllSheltersAsync()
        {
            return await _context.Shelters.OrderBy(s => s.Name).ToListAsync();
        }

        public async Task<Shelter> GetShelterByIdAsync(long id)
        {
            return await _context.Shelters.FindAsync(id);
        }

        public async Task<Shelter> UpdateShelterAsync(long id, Shelter updatedShelter)
        {
            var existingShelter = await _context.Shelters.FindAsync(id);
            if (existingShelter == null)
                return null;

            if (updatedShelter.Capacity <= 0)
                throw new ArgumentException("Capacidade deve ser maior que zero");

            if (updatedShelter.CurrentOccupancy < 0)
                updatedShelter.CurrentOccupancy = 0;

            if (updatedShelter.CurrentOccupancy > updatedShelter.Capacity)
                throw new ArgumentException("Ocupação atual não pode ser maior que a capacidade");

            existingShelter.Name = updatedShelter.Name;
            existingShelter.Address = updatedShelter.Address;
            existingShelter.Capacity = updatedShelter.Capacity;
            existingShelter.CurrentOccupancy = updatedShelter.CurrentOccupancy;
            existingShelter.Latitude = updatedShelter.Latitude;
            existingShelter.Longitude = updatedShelter.Longitude;

            await _context.SaveChangesAsync();
            return existingShelter;
        }

        public async Task<bool> DeleteShelterAsync(long id)
        {
            var shelter = await _context.Shelters.FindAsync(id);
            if (shelter == null)
                return false;

            _context.Shelters.Remove(shelter);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCapacityAsync(long id, int newCurrentOccupancy)
        {
            var shelter = await _context.Shelters.FindAsync(id);
            if (shelter == null)
                return false;

            if (newCurrentOccupancy < 0 || newCurrentOccupancy > shelter.Capacity)
                throw new ArgumentException("Ocupação inválida");

            shelter.CurrentOccupancy = newCurrentOccupancy;
            
            await _context.SaveChangesAsync();
            return true;
        }

        // formula de haversine
        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // raio da terra em km

            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

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