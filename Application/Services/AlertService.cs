using SCED.API.Application.Interfaces;
using SCED.API.Domain.Entity;
using SCED.API.Domain.Enums;
using SCED.API.Domain.Interfaces;

namespace SCED.API.Application.Services
{
    public class AlertService : IAlertService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AlertService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<IEnumerable<Alert>> GetAllAlertsAsync()
        {
            try
            {
                return await _unitOfWork.Alerts.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao buscar todos os alertas.", ex);
            }
        }

        public async Task<Alert?> GetAlertByIdAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentException("O ID do alerta deve ser maior que zero.", nameof(id));

            try
            {
                return await _unitOfWork.Alerts.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao buscar alerta com ID {id}.", ex);
            }
        }

        public async Task<IEnumerable<Alert>> GetAlertsByTypeAsync(AlertType type)
        {
            if (!Enum.IsDefined(typeof(AlertType), type))
                throw new ArgumentException("Tipo de alerta inválido.", nameof(type));

            try
            {
                return await _unitOfWork.Alerts.GetAlertsByTypeAsync(type);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao buscar alertas do tipo {type}.", ex);
            }
        }

        public async Task<IEnumerable<Alert>> GetAlertsBySeverityAsync(int severity)
        {
            if (severity < 1 || severity > 10)
                throw new ArgumentException("A severidade deve estar entre 1 e 10.", nameof(severity));

            try
            {
                return await _unitOfWork.Alerts.GetAlertsBySeverityAsync(severity);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao buscar alertas com severidade {severity}.", ex);
            }
        }

        public async Task<IEnumerable<Alert>> GetAlertsInRadiusAsync(double latitude, double longitude, double radiusKm = 10.0)
        {
            if (latitude < -90 || latitude > 90)
                throw new ArgumentException("A latitude deve estar entre -90 e 90 graus.", nameof(latitude));

            if (longitude < -180 || longitude > 180)
                throw new ArgumentException("A longitude deve estar entre -180 e 180 graus.", nameof(longitude));

            if (radiusKm <= 0 || radiusKm > 1000)
                throw new ArgumentException("O raio deve estar entre 0 e 1000 km.", nameof(radiusKm));

            try
            {
                return await _unitOfWork.Alerts.GetAlertsInRadiusAsync(latitude, longitude, radiusKm);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao buscar alertas na área especificada.", ex);
            }
        }

        public async Task<IEnumerable<Alert>> GetRecentAlertsAsync(DateTime since)
        {
            if (since > DateTime.UtcNow)
                throw new ArgumentException("A data de referência não pode ser futura.", nameof(since));

            if (since < DateTime.UtcNow.AddYears(-1))
                throw new ArgumentException("A data de referência não pode ser anterior a 1 ano.", nameof(since));

            try
            {
                return await _unitOfWork.Alerts.GetRecentAlertsAsync(since);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao buscar alertas recentes.", ex);
            }
        }

        public async Task<Alert> CreateAlertAsync(Alert alert)
        {
            if (alert == null)
                throw new ArgumentNullException(nameof(alert), "O alerta não pode ser nulo.");

            ValidateAlert(alert);

            try
            {
                if (alert.Timestamp == default)
                    alert.Timestamp = DateTime.UtcNow;

                await _unitOfWork.Alerts.AddAsync(alert);
                await _unitOfWork.SaveChangesAsync();
                return alert;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Erro ao criar o alerta.", ex);
            }
        }

        public async Task<Alert?> UpdateAlertAsync(long id, Alert updatedAlert)
        {
            if (id <= 0)
                throw new ArgumentException("O ID do alerta deve ser maior que zero.", nameof(id));

            if (updatedAlert == null)
                throw new ArgumentNullException(nameof(updatedAlert), "O alerta atualizado não pode ser nulo.");

            if (id != updatedAlert.Id)
                throw new ArgumentException("O ID fornecido não corresponde ao ID do alerta.", nameof(id));

            ValidateAlert(updatedAlert);

            try
            {
                Alert existingAlert = await _unitOfWork.Alerts.GetByIdAsync(id);
                if (existingAlert == null) 
                    return null;

                existingAlert.Type = updatedAlert.Type;
                existingAlert.Severity = updatedAlert.Severity;
                existingAlert.Timestamp = updatedAlert.Timestamp;
                existingAlert.Description = updatedAlert.Description;
                existingAlert.Longitude = updatedAlert.Longitude;
                existingAlert.Latitude = updatedAlert.Latitude;

                await _unitOfWork.Alerts.UpdateAsync(existingAlert);
                await _unitOfWork.SaveChangesAsync();
                return existingAlert;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao atualizar o alerta com ID {id}.", ex);
            }
        }

        public async Task<bool> DeleteAlertAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentException("O ID do alerta deve ser maior que zero.", nameof(id));

            try
            {
                Alert alert = await _unitOfWork.Alerts.GetByIdAsync(id);
                if (alert == null) 
                    return false;

                await _unitOfWork.Alerts.DeleteAsync(alert);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao deletar o alerta com ID {id}.", ex);
            }
        }

        private static void ValidateAlert(Alert alert)
        {
            if (string.IsNullOrWhiteSpace(alert.Description))
                throw new ArgumentException("A descrição do alerta é obrigatória.", nameof(alert.Description));

            if (alert.Description.Length > 1000)
                throw new ArgumentException("A descrição do alerta não pode exceder 1000 caracteres.", nameof(alert.Description));

            if (!Enum.IsDefined(typeof(AlertType), alert.Type))
                throw new ArgumentException("Tipo de alerta inválido.", nameof(alert.Type));

            if (alert.Severity < 1 || alert.Severity > 5)
                throw new ArgumentException("A severidade deve estar entre 1 e 5.", nameof(alert.Severity));

            if (alert.Latitude < -90 || alert.Latitude > 90)
                throw new ArgumentException("A latitude deve estar entre -90 e 90 graus.", nameof(alert.Latitude));

            if (alert.Longitude < -180 || alert.Longitude > 180)
                throw new ArgumentException("A longitude deve estar entre -180 e 180 graus.", nameof(alert.Longitude));

            if (alert.Timestamp > DateTime.UtcNow.AddHours(1))
                throw new ArgumentException("O timestamp não pode ser muito distante no futuro.", nameof(alert.Timestamp));

            if (alert.Timestamp < DateTime.UtcNow.AddYears(-1))
                throw new ArgumentException("O timestamp não pode ser anterior a 1 ano.", nameof(alert.Timestamp));
        }
    }
}