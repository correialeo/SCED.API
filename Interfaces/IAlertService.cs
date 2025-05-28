using SCED.API.Domain.Entity;
using SCED.API.Domain.Enums;

namespace SCED.API.Interfaces
{
    public interface IAlertService
    {
        Task<IEnumerable<Alert>> GetAllAlertsAsync();
        Task<Alert> GetAlertByIdAsync(long id);
        Task<Alert> UpdateAlertAsync(long id, Alert updatedAlert);
        Task<Alert> CreateAlertAsync(Alert alert);
        Task<bool> DeleteAlertAsync(long id);
        Task<IEnumerable<Alert>> GetAlertsByTypeAsync(AlertType type);
        Task<IEnumerable<Alert>> GetAlertsBySeverityAsync(int severity);
        Task<IEnumerable<Alert>> GetAlertsInRadiusAsync(double latitude, double longitude, double radiusKm);
        Task<IEnumerable<Alert>> GetRecentAlertsAsync(DateTime since);
    }
}