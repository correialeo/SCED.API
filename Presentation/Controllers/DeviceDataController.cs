using Microsoft.AspNetCore.Mvc;
using SCED.API.Domain.Entity;
using SCED.API.Application.Services;
using SCED.API.Presentation.DTO;

namespace SCED.API.Presentation.Controllers
{
    /// <summary>
    /// Controller responsável pelo recebimento e consulta de dados de dispositivos
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class DeviceDataController : ControllerBase
    {
        private readonly DeviceDataService _deviceDataService;

        /// <summary>
        /// Construtor do DeviceDataController
        /// </summary>
        /// <param name="deviceDataService">Serviço de dados de dispositivos</param>
        public DeviceDataController(DeviceDataService deviceDataService)
        {
            _deviceDataService = deviceDataService ?? throw new ArgumentNullException(nameof(deviceDataService));
        }

        /// <summary>
        /// Recebe dados de um dispositivo e processa alertas automaticamente
        /// </summary>
        /// <param name="deviceDataDTO">Dados do dispositivo a serem recebidos</param>
        /// <returns>Dados do dispositivo recebidos com status da operação</returns>
        /// <response code="200">Dados recebidos</response>
        /// <response code="400">Dados inválidos ou dispositivo não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(DeviceData), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DeviceData>> ReceiveData([FromBody] DeviceDataDTO deviceDataDTO)
        {
            try
            {
                var deviceData = await _deviceDataService.ReceiveDataAsync(deviceDataDTO);
                return Ok(deviceData);
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
        /// Obtém todos os dados registrados de um dispositivo específico
        /// </summary>
        /// <param name="deviceId">ID do dispositivo</param>
        /// <returns>Coleção de dados do dispositivo</returns>
        /// <response code="200">Retorna a coleção de dados</response>
        /// <response code="400">ID do dispositivo inválido</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{deviceId:long}")]
        [ProducesResponseType(typeof(IEnumerable<DeviceData>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<DeviceData>>> GetDataByDeviceId(long deviceId)
        {
            try
            {
                var data = await _deviceDataService.GetDataByDeviceIdAsync(deviceId);
                return Ok(data);
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
        /// Obtém todos os dados de todos os dispositivos
        /// </summary>
        /// <returns>Coleção de todos os dados dos dispositivos</returns>
        /// <response code="200">Retorna todos os dados</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DeviceData>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<DeviceData>>> GetAllData()
        {
            try
            {
                IEnumerable<DeviceData>? data = await _deviceDataService.GetAllDataAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }
    }
}