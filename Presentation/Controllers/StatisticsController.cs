using Microsoft.AspNetCore.Mvc;
using SCED.API.Application.Services;
using SCED.API.Presentation.DTO;
using System.ComponentModel.DataAnnotations;

namespace SCED.API.Presentation.Controllers
{
    /// <summary>
    /// Controlador responsável por fornecer estatísticas e dados analíticos do sistema
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class StatisticsController : ControllerBase
    {
        private readonly StatisticsService _statisticsService;

        public StatisticsController(StatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        /// <summary>
        /// Obtém as estatísticas completas do dashboard
        /// </summary>
        /// <param name="fromDate">Data inicial para o período de análise. Se não informada, será considerado 3 meses atrás</param>
        /// <param name="toDate">Data final para o período de análise. Se não informada, será considerada a data atual</param>
        /// <returns>Estatísticas consolidadas do dashboard</returns>
        /// <response code="200">Estatísticas obtidas com sucesso</response>
        /// <response code="400">Parâmetros de data inválidos</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(DashboardStatisticsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DashboardStatisticsDTO>> GetDashboardStatistics(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var statistics = await _statisticsService.GetDashboardStatisticsAsync(fromDate, toDate);
                return Ok(statistics);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Obtém estatísticas de alertas agrupadas por localização geográfica
        /// </summary>
        /// <param name="fromDate">Data inicial para o período de análise. Se não informada, será considerado 3 meses atrás</param>
        /// <param name="toDate">Data final para o período de análise. Se não informada, será considerada a data atual</param>
        /// <param name="radiusKm">Raio em quilômetros para filtrar por proximidade (opcional)</param>
        /// <param name="centerLat">Latitude do centro para filtro por proximidade (obrigatória se radiusKm informado)</param>
        /// <param name="centerLng">Longitude do centro para filtro por proximidade (obrigatória se radiusKm informado)</param>
        /// <returns>Lista de estatísticas por localização ordenada por score de risco</returns>
        /// <response code="200">Estatísticas por localização obtidas com sucesso</response>
        /// <response code="400">Parâmetros inválidos</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("locations")]
        [ProducesResponseType(typeof(List<LocationStatisticsDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<LocationStatisticsDTO>>> GetLocationStatistics(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery, Range(0.1, double.MaxValue)] double? radiusKm = null,
            [FromQuery, Range(-90, 90)] double? centerLat = null,
            [FromQuery, Range(-180, 180)] double? centerLng = null)
        {
            try
            {
                var from = fromDate ?? DateTime.UtcNow.AddMonths(-3);
                var to = toDate ?? DateTime.UtcNow;

                var statistics = await _statisticsService.GetLocationStatisticsAsync(from, to, radiusKm, centerLat, centerLng);
                return Ok(statistics);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Obtém estatísticas dos dispositivos agrupadas por tipo
        /// </summary>
        /// <param name="fromDate">Data inicial para o período de análise. Se não informada, será considerado 1 mês atrás</param>
        /// <param name="toDate">Data final para o período de análise. Se não informada, será considerada a data atual</param>
        /// <returns>Lista de estatísticas por tipo de dispositivo</returns>
        /// <response code="200">Estatísticas por tipo de dispositivo obtidas com sucesso</response>
        /// <response code="400">Parâmetros de data inválidos</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("device-types")]
        [ProducesResponseType(typeof(List<DeviceTypeStatisticsDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<DeviceTypeStatisticsDTO>>> GetDeviceTypeStatistics(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var from = fromDate ?? DateTime.UtcNow.AddMonths(-1);
                var to = toDate ?? DateTime.UtcNow;

                var statistics = await _statisticsService.GetDeviceTypeStatisticsAsync(from, to);
                return Ok(statistics);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Obtém as tendências temporais dos alertas
        /// </summary>
        /// <param name="fromDate">Data inicial para o período de análise. Se não informada, será considerado 3 meses atrás</param>
        /// <param name="toDate">Data final para o período de análise. Se não informada, será considerada a data atual</param>
        /// <returns>Lista de tendências de alertas agrupadas por data e tipo</returns>
        /// <response code="200">Tendências de alertas obtidas com sucesso</response>
        /// <response code="400">Parâmetros de data inválidos</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("alert-trends")]
        [ProducesResponseType(typeof(List<AlertTrendDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<AlertTrendDTO>>> GetAlertTrends(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var from = fromDate ?? DateTime.UtcNow.AddMonths(-3);
                var to = toDate ?? DateTime.UtcNow;

                var trends = await _statisticsService.GetAlertTrendsAsync(from, to);
                return Ok(trends);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Obtém os hotspots geográficos com maior concentração de alertas
        /// </summary>
        /// <param name="fromDate">Data inicial para o período de análise. Se não informada, será considerado 3 meses atrás</param>
        /// <param name="toDate">Data final para o período de análise. Se não informada, será considerada a data atual</param>
        /// <param name="topN">Número máximo de hotspots a retornar (padrão: 10)</param>
        /// <returns>Lista dos principais hotspots geográficos ordenados por quantidade de alertas</returns>
        /// <response code="200">Hotspots geográficos obtidos com sucesso</response>
        /// <response code="400">Parâmetros inválidos</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("hotspots")]
        [ProducesResponseType(typeof(List<GeographicHotspotDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<GeographicHotspotDTO>>> GetGeographicHotspots(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery, Range(1, 100)] int topN = 10)
        {
            try
            {
                var from = fromDate ?? DateTime.UtcNow.AddMonths(-3);
                var to = toDate ?? DateTime.UtcNow;

                var hotspots = await _statisticsService.GetGeographicHotspotsAsync(from, to, topN);
                return Ok(hotspots);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Atualiza as estatísticas em tempo real com novos dados de dispositivo
        /// </summary>
        /// <param name="deviceData">Dados do dispositivo para atualização</param>
        /// <returns>Confirmação da atualização</returns>
        /// <response code="200">Estatísticas atualizadas com sucesso</response>
        /// <response code="400">Dados do dispositivo inválidos</response>
        /// <response code="404">Dispositivo não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("realtime-update")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateRealTimeStatistics(
            [FromBody, Required] DeviceDataDTO deviceData)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Dados do dispositivo são obrigatórios");
                }

                await _statisticsService.UpdateRealTimeStatisticsAsync(deviceData.DeviceId, deviceData.Value);
                return Ok("Estatísticas atualizadas em tempo real com sucesso");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("não encontrado"))
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}