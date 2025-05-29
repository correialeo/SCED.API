using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCED.API.Application.Interfaces;
using SCED.API.Domain.Entity;
using SCED.API.Domain.Enums;

namespace SCED.API.Presentation.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de alertas do sistema SCED
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AlertsController : ControllerBase
    {
        private readonly IAlertService _alertService;

        /// <summary>
        /// Construtor do AlertsController
        /// </summary>
        /// <param name="alertService">Serviço de alertas</param>
        public AlertsController(IAlertService alertService)
        {
            _alertService = alertService ?? throw new ArgumentNullException(nameof(alertService));
        }

        /// <summary>
        /// Obtém todos os alertas cadastrados no sistema
        /// </summary>
        /// <returns>Lista de todos os alertas</returns>
        /// <response code="200">Retorna a lista de alertas</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Alert>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Alert>>> GetAlerts()
        {
            try
            {
                IEnumerable<Alert> alerts = await _alertService.GetAllAlertsAsync();
                return Ok(alerts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém um alerta específico pelo seu ID
        /// </summary>
        /// <param name="id">ID do alerta</param>
        /// <returns>Dados do alerta solicitado</returns>
        /// <response code="200">Retorna o alerta encontrado</response>
        /// <response code="400">ID inválido</response>
        /// <response code="404">Alerta não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(Alert), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Alert>> GetAlert(long id)
        {
            try
            {
                Alert alert = await _alertService.GetAlertByIdAsync(id);
                return alert != null ? Ok(alert) : NotFound($"Alerta com ID {id} não foi encontrado");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém alertas filtrados por tipo
        /// </summary>
        /// <param name="type">Tipo do alerta (ex: Fire, Flood, etc.)</param>
        /// <returns>Lista de alertas do tipo especificado</returns>
        /// <response code="200">Retorna a lista de alertas do tipo especificado</response>
        /// <response code="400">Tipo de alerta inválido</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("type/{type}")]
        [ProducesResponseType(typeof(IEnumerable<Alert>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Alert>>> GetAlertsByType(AlertType type)
        {
            try
            {
                IEnumerable<Alert> alerts = await _alertService.GetAlertsByTypeAsync(type);
                return Ok(alerts);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém alertas filtrados por severidade
        /// </summary>
        /// <param name="severity">Nível de severidade (1-10)</param>
        /// <returns>Lista de alertas com a severidade especificada</returns>
        /// <response code="200">Retorna a lista de alertas com a severidade especificada</response>
        /// <response code="400">Severidade inválida (deve estar entre 1 e 10)</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("severity/{severity:int}")]
        [ProducesResponseType(typeof(IEnumerable<Alert>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Alert>>> GetAlertsBySeverity(int severity)
        {
            try
            {
                IEnumerable<Alert> alerts = await _alertService.GetAlertsBySeverityAsync(severity);
                return Ok(alerts);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém alertas dentro de um raio específico de uma localização
        /// </summary>
        /// <param name="latitude">Latitude da localização central</param>
        /// <param name="longitude">Longitude da localização central</param>
        /// <param name="radiusKm">Raio de busca em quilômetros (padrão: 10km, máximo: 1000km)</param>
        /// <returns>Lista de alertas dentro do raio especificado</returns>
        /// <response code="200">Retorna a lista de alertas na área especificada</response>
        /// <response code="400">Parâmetros de localização ou raio inválidos</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("radius")]
        [ProducesResponseType(typeof(IEnumerable<Alert>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Alert>>> GetAlertsInRadius(
            [FromQuery] double latitude, 
            [FromQuery] double longitude, 
            [FromQuery] double radiusKm = 10.0)
        {
            try
            {
                IEnumerable<Alert> alerts = await _alertService.GetAlertsInRadiusAsync(latitude, longitude, radiusKm);
                return Ok(alerts);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém alertas criados a partir de uma data específica
        /// </summary>
        /// <param name="since">Data de referência (formato ISO 8601)</param>
        /// <returns>Lista de alertas criados após a data especificada</returns>
        /// <response code="200">Retorna a lista de alertas recentes</response>
        /// <response code="400">Data inválida</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("recent")]
        [ProducesResponseType(typeof(IEnumerable<Alert>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Alert>>> GetRecentAlerts([FromQuery] DateTime since)
        {
            try
            {
                IEnumerable<Alert> alerts = await _alertService.GetRecentAlertsAsync(since);
                return Ok(alerts);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Cria um novo alerta no sistema
        /// </summary>
        /// <param name="alert">Dados do alerta a ser criado</param>
        /// <returns>Dados do alerta criado</returns>
        /// <response code="201">Alerta criado com sucesso</response>
        /// <response code="400">Dados do alerta inválidos</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(Alert), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize (Roles = "Administrator, Authority")]
        public async Task<ActionResult<Alert>> PostAlert([FromBody] Alert alert)
        {
            try
            {
                Alert createdAlert = await _alertService.CreateAlertAsync(alert);
                return CreatedAtAction(nameof(GetAlert), new { id = createdAlert.Id }, createdAlert);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza um alerta existente
        /// </summary>
        /// <param name="id">ID do alerta a ser atualizado</param>
        /// <param name="alert">Dados atualizados do alerta</param>
        /// <returns>Confirmação da atualização</returns>
        /// <response code="204">Alerta atualizado com sucesso</response>
        /// <response code="400">Dados inválidos ou ID não corresponde</response>
        /// <response code="404">Alerta não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize (Roles = "Administrator, Authority")]
        public async Task<IActionResult> PutAlert(long id, [FromBody] Alert alert)
        {
            try
            {
                Alert updatedAlert = await _alertService.UpdateAlertAsync(id, alert);
                return updatedAlert != null ? NoContent() : NotFound($"Alerta com ID {id} não foi encontrado");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Remove um alerta do sistema
        /// </summary>
        /// <param name="id">ID do alerta a ser removido</param>
        /// <returns>Confirmação da remoção</returns>
        /// <response code="204">Alerta removido com sucesso</response>
        /// <response code="400">ID inválido</response>
        /// <response code="404">Alerta não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize (Roles = "Administrator, Authority")]
        public async Task<IActionResult> DeleteAlert(long id)
        {
            try
            {
                bool deleted = await _alertService.DeleteAlertAsync(id);
                return deleted ? NoContent() : NotFound($"Alerta com ID {id} não foi encontrado");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }
    }
}