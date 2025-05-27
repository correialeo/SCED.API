// AlertService.cs
using Microsoft.EntityFrameworkCore;
using SCED.API.Domain.Entity;
using SCED.API.Infrasctructure.Context;
using SCED.API.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCED.API.Services
{
    public class AlertService : IAlertService
    {
        private readonly DatabaseContext _context;

        public AlertService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Alert>> GetAllAlertsAsync()
        {
            return await _context.Alerts.ToListAsync();
        }

        public async Task<Alert> GetAlertByIdAsync(long id)
        {
            return await _context.Alerts.FindAsync(id);
        }

        public async Task<Alert> UpdateAlertAsync(long id, Alert updatedAlert)
        {
            var alert = await _context.Alerts.FindAsync(id);
            if (alert == null) return null;

            alert.Type = updatedAlert.Type;
            alert.Severity = updatedAlert.Severity;
            alert.Timestamp = updatedAlert.Timestamp;
            alert.Description = updatedAlert.Description;
            alert.Longitude = updatedAlert.Longitude;
            alert.Latitude = updatedAlert.Latitude;

            await _context.SaveChangesAsync();
            return alert;
        }

        public async Task<Alert> CreateAlertAsync(Alert alert)
        {
            _context.Alerts.Add(alert);
            await _context.SaveChangesAsync();
            return alert;
        }

        public async Task<bool> DeleteAlertAsync(long id)
        {
            var alert = await _context.Alerts.FindAsync(id);
            if (alert == null) return false;

            _context.Alerts.Remove(alert);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}