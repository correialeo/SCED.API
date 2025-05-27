using Microsoft.AspNetCore.Mvc;
using SCED.API.Domain.Entity;
using SCED.API.Interfaces;

namespace SCED.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlertsController : ControllerBase
    {
        private readonly IAlertService _alertService;

        public AlertsController(IAlertService alertService)
        {
            _alertService = alertService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Alert>>> GetAlerts()
        {
            var alerts = await _alertService.GetAllAlertsAsync();
            return Ok(alerts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Alert>> GetAlert(long id)
        {
            var alert = await _alertService.GetAlertByIdAsync(id);
            return alert != null ? Ok(alert) : NotFound();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutAlert(long id, Alert alert)
        {
            if (id != alert.Id)
                return BadRequest("ID do alerta não confere");

            try
            {
                var updatedAlert = await _alertService.UpdateAlertAsync(id, alert);
                return updatedAlert != null ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar alerta: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Alert>> PostAlert(Alert alert)
        {
            try
            {
                var createdAlert = await _alertService.CreateAlertAsync(alert);
                return CreatedAtAction("GetAlert", new { id = createdAlert.Id }, createdAlert);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar alerta: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlert(long id)
        {
            var deleted = await _alertService.DeleteAlertAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}