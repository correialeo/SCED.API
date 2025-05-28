using SCED.API.Domain.Entity;
using SCED.API.Domain.Enums;

namespace SCED.API.Domain.Interfaces
{
    public interface IAlertRepository : IRepository<Alert>
    {
        Task<IEnumerable<Alert>> GetAlertsByTypeAsync(AlertType type);
        Task<IEnumerable<Alert>> GetAlertsBySeverityAsync(int severity);
        Task<IEnumerable<Alert>> GetAlertsInRadiusAsync(double latitude, double longitude, double radiusKm);
        Task<IEnumerable<Alert>> GetRecentAlertsAsync(DateTime since);
    }
}