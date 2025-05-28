using Microsoft.EntityFrameworkCore;
using SCED.API.Domain.Entity;
using SCED.API.Domain.Enums;
using SCED.API.Domain.Interfaces;
using SCED.API.Infrasctructure.Context;

namespace SCED.API.Infrastructure.Repositories
{
    public class AlertRepository : Repository<Alert>, IAlertRepository
    {
        public AlertRepository(DatabaseContext context) : base(context) { }

        public async Task<IEnumerable<Alert>> GetAlertsByTypeAsync(AlertType type)
        {
            return await _dbSet.Where(a => a.Type == type).ToListAsync();
        }

        public async Task<IEnumerable<Alert>> GetAlertsBySeverityAsync(int severity)
        {
            return await _dbSet.Where(a => a.Severity == severity).ToListAsync();
        }

        public async Task<IEnumerable<Alert>> GetAlertsInRadiusAsync(double latitude, double longitude, double radiusKm)
        {
            var alerts = await _dbSet.ToListAsync();
            return alerts.Where(alert => 
                CalculateDistance(latitude, longitude, alert.Latitude, alert.Longitude) <= radiusKm
            ).OrderBy(alert => 
                CalculateDistance(latitude, longitude, alert.Latitude, alert.Longitude)
            );
        }

        public async Task<IEnumerable<Alert>> GetRecentAlertsAsync(DateTime since)
        {
            return await _dbSet
                .Where(a => a.Timestamp >= since)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
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