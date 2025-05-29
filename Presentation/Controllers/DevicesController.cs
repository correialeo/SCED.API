using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCED.API.Application.Interfaces;
using SCED.API.Domain.Entity;
using SCED.API.Domain.Enums;
using SCED.API.Presentation.DTO;

namespace SCED.API.Presentation.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de dispositivos do sistema SCED
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize(Roles = "Administrator, Authority")]
    public class DevicesController : ControllerBase
    {
        private readonly IDeviceService _deviceService;

        /// <summary>
        /// Construtor do DevicesController
        /// </summary>
        /// <param name="deviceService">Serviço de dispositivos</param>
        public DevicesController(IDeviceService deviceService)
        {
            _deviceService = deviceService ?? throw new ArgumentNullException(nameof(deviceService));
        }

        /// <summary>
        /// Obtém todos os dispositivos cadastrados no sistema
        /// </summary>
        /// <returns>Lista de todos os dispositivos</returns>
        /// <response code="200">Retorna a lista de dispositivos</response>
        /// <response code="401">Token de autenticação inválido ou ausente</response>
        /// <response code="403">Usuário não possui permissão para acessar este recurso</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Device>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Device>>> GetDevices()
        {
            try
            {
                IEnumerable<Device> devices = await _deviceService.GetAllDevicesAsync();
                return Ok(devices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém um dispositivo específico pelo seu ID
        /// </summary>
        /// <param name="id">ID do dispositivo</param>
        /// <returns>Dados do dispositivo solicitado</returns>
        /// <response code="200">Retorna o dispositivo encontrado</response>
        /// <response code="400">ID inválido</response>
        /// <response code="401">Token de autenticação inválido ou ausente</response>
        /// <response code="403">Usuário não possui permissão para acessar este recurso</response>
        /// <response code="404">Dispositivo não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(Device), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Device>> GetDevice(long id)
        {
            try
            {
                Device device = await _deviceService.GetDeviceByIdAsync(id);
                return device != null ? Ok(device) : NotFound($"Dispositivo com ID {id} não foi encontrado");
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
        /// Obtém um dispositivo com seus dados históricos pelo ID
        /// </summary>
        /// <param name="id">ID do dispositivo</param>
        /// <returns>Dispositivo com dados históricos ou NotFound se não existir</returns>
        /// <response code="200">Retorna o dispositivo com dados históricos</response>
        /// <response code="400">ID do dispositivo inválido</response>
        /// <response code="401">Token de autenticação inválido ou ausente</response>
        /// <response code="403">Usuário não possui permissão para acessar este recurso</response>
        /// <response code="404">Dispositivo não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{id:long}/with-data")]
        [ProducesResponseType(typeof(Device), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Device>> GetDeviceWithData(long id)
        {
            try
            {
                Device device = await _deviceService.GetDeviceWithDataAsync(id);
                return device != null ? Ok(device) : NotFound($"Dispositivo com ID {id} não foi encontrado");
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
        /// Obtém dispositivos filtrados por tipo
        /// </summary>
        /// <param name="type">Tipo do dispositivo (ex: TemperatureSensor, WaterLevelSensor)</param>
        /// <returns>Lista de dispositivos do tipo especificado</returns>
        /// <response code="200">Retorna a lista de dispositivos do tipo especificado</response>
        /// <response code="400">Tipo de dispositivo inválido</response>
        /// <response code="401">Token de autenticação inválido ou ausente</response>
        /// <response code="403">Usuário não possui permissão para acessar este recurso</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("type/{type}")]
        [ProducesResponseType(typeof(IEnumerable<Device>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Device>>> GetDevicesByType(DeviceType type)
        {
            try
            {
                IEnumerable<Device> devices = await _deviceService.GetDevicesByTypeAsync(type);
                return Ok(devices);
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
        /// Obtém dispositivos filtrados por status
        /// </summary>
        /// <param name="status">Status do dispositivo (ex: Active, Inactive)</param>
        /// <returns>Lista de dispositivos com o status especificado</returns>
        /// <response code="200">Retorna a lista de dispositivos com o status especificado</response>
        /// <response code="400">Status do dispositivo inválido</response>
        /// <response code="401">Token de autenticação inválido ou ausente</response>
        /// <response code="403">Usuário não possui permissão para acessar este recurso</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(IEnumerable<Device>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Device>>> GetDevicesByStatus(DeviceStatus status)
        {
            try
            {
                IEnumerable<Device> devices = await _deviceService.GetDevicesByStatusAsync(status);
                return Ok(devices);
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
        /// Obtém dispositivos dentro de um raio específico de uma localização
        /// </summary>
        /// <param name="latitude">Latitude da localização central</param>
        /// <param name="longitude">Longitude da localização central</param>
        /// <param name="radiusKm">Raio de busca em quilômetros (padrão: 5km, máximo: 1000km)</param>
        /// <returns>Lista de dispositivos dentro do raio especificado</returns>
        /// <response code="200">Retorna a lista de dispositivos na área especificada</response>
        /// <response code="400">Parâmetros de localização ou raio inválidos</response>
        /// <response code="401">Token de autenticação inválido ou ausente</response>
        /// <response code="403">Usuário não possui permissão para acessar este recurso</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("radius")]
        [ProducesResponseType(typeof(IEnumerable<Device>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Device>>> GetDevicesInRadius(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] double radiusKm = 5.0)
        {
            try
            {
                IEnumerable<Device> devices = await _deviceService.GetDevicesInRadiusAsync(latitude, longitude, radiusKm);
                return Ok(devices);
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
        /// Cria um novo dispositivo no sistema
        /// </summary>
        /// <param name="deviceDTO">Dados do dispositivo a ser criado</param>
        /// <returns>Dados do dispositivo criado</returns>
        /// <response code="201">Dispositivo criado com sucesso</response>
        /// <response code="400">Dados do dispositivo inválidos</response>
        /// <response code="401">Token de autenticação inválido ou ausente</response>
        /// <response code="403">Usuário não possui permissão para acessar este recurso</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(Device), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Device>> PostDevice([FromBody] DeviceDTO deviceDTO)
        {
            if (deviceDTO == null)
                return BadRequest("Dados do dispositivo não podem ser nulos.");

            try
            {
                var device = new Device
                {
                    Type = deviceDTO.Type,
                    Status = deviceDTO.Status,
                    Latitude = deviceDTO.Latitude,
                    Longitude = deviceDTO.Longitude,
                };

                Device createdDevice = await _deviceService.CreateDeviceAsync(device);
                return CreatedAtAction(nameof(GetDevice), new { id = createdDevice.Id }, createdDevice);
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
        /// Atualiza um dispositivo existente
        /// </summary>
        /// <param name="id">ID do dispositivo a ser atualizado</param>
        /// <param name="deviceDTO">Dados atualizados do dispositivo</param>
        /// <returns>Confirmação da atualização</returns>
        /// <response code="204">Dispositivo atualizado com sucesso</response>
        /// <response code="400">Dados inválidos ou ID não corresponde</response>
        /// <response code="401">Token de autenticação inválido ou ausente</response>
        /// <response code="403">Usuário não possui permissão para acessar este recurso</response>
        /// <response code="404">Dispositivo não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutDevice(long id, [FromBody] DeviceDTO deviceDTO)
        {
            if (deviceDTO == null)
                return BadRequest("Dados do dispositivo não podem ser nulos.");

            try
            {
                var device = new Device
                {
                    Id = id,
                    Type = deviceDTO.Type,
                    Status = deviceDTO.Status,
                    Latitude = deviceDTO.Latitude,
                    Longitude = deviceDTO.Longitude
                };

                Device updatedDevice = await _deviceService.UpdateDeviceAsync(id, device);
                return updatedDevice != null ? NoContent() : NotFound($"Dispositivo com ID {id} não foi encontrado");
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
        /// Remove um dispositivo do sistema
        /// </summary>
        /// <param name="id">ID do dispositivo a ser removido</param>
        /// <returns>Confirmação da remoção</returns>
        /// <response code="204">Dispositivo removido com sucesso</response>
        /// <response code="400">ID inválido</response>
        /// <response code="401">Token de autenticação inválido ou ausente</response>
        /// <response code="403">Usuário não possui permissão para acessar este recurso</response>
        /// <response code="404">Dispositivo não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteDevice(long id)
        {
            try
            {
                bool deleted = await _deviceService.DeleteDeviceAsync(id);
                return deleted ? NoContent() : NotFound($"Dispositivo com ID {id} não foi encontrado");
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