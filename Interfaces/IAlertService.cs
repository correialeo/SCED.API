using SCED.API.Domain.Entity;

namespace SCED.API.Interfaces
{
    public interface IAlertService
    {
        Task<IEnumerable<Alert>> GetAllAlertsAsync();
        Task<Alert> GetAlertByIdAsync(long id);
        Task<Alert> UpdateAlertAsync(long id, Alert updatedAlert);
        Task<Alert> CreateAlertAsync(Alert alert);
        Task<bool> DeleteAlertAsync(long id);
    }
}