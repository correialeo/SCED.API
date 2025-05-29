using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCED.API.Domain.Entity;
using SCED.API.Domain.Enums;
using SCED.API.Interfaces;
using System;
using System.Threading.Tasks;

namespace SCED.API.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de recursos do sistema SCED
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize]
    public class ResourcesController : ControllerBase
    {
        private readonly IResourceService _resourceService;

        /// <summary>
        /// Construtor do ResourcesController
        /// </summary>
        /// <param name="resourceService">Serviço de recursos</param>
        public ResourcesController(IResourceService resourceService)
        {
            _resourceService = resourceService ?? throw new ArgumentNullException(nameof(resourceService));
        }

        /// <summary>
        /// Obtém todos os recursos cadastrados no sistema
        /// </summary>
        /// <returns>Lista de todos os recursos</returns>
        /// <response code="200">Retorna a lista de recursos</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Resource>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Resource>>> GetResources()
        {
            try
            {
                IEnumerable<Resource> resources = await _resourceService.GetAllResourcesAsync();
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém um recurso específico pelo seu ID
        /// </summary>
        /// <param name="id">ID do recurso</param>
        /// <returns>Dados do recurso solicitado</returns>
        /// <response code="200">Retorna o recurso encontrado</response>
        /// <response code="400">ID inválido</response>
        /// <response code="404">Recurso não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(Resource), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Resource>> GetResource(long id)
        {
            try
            {
                Resource resource = await _resourceService.GetResourceByIdAsync(id);
                return resource != null ? Ok(resource) : NotFound($"Recurso com ID {id} não foi encontrado");
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
        /// Obtém recursos filtrados por tipo
        /// </summary>
        /// <param name="type">Tipo do recurso</param>
        /// <returns>Lista de recursos do tipo especificado</returns>
        /// <response code="200">Retorna a lista de recursos do tipo especificado</response>
        /// <response code="400">Tipo de recurso inválido</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("type/{type}")]
        [ProducesResponseType(typeof(IEnumerable<Resource>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Resource>>> GetResourcesByType(ResourceType type)
        {
            try
            {
                IEnumerable<Resource> resources = await _resourceService.GetResourcesByTypeAsync(type);
                return Ok(resources);
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
        /// Obtém recursos disponíveis para alocação
        /// </summary>
        /// <returns>Lista de recursos disponíveis</returns>
        /// <response code="200">Retorna a lista de recursos disponíveis</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("available")]
        [ProducesResponseType(typeof(IEnumerable<Resource>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Resource>>> GetAvailableResources()
        {
            try
            {
                IEnumerable<Resource> resources = await _resourceService.GetAvailableResourcesAsync();
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém recursos filtrados por status
        /// </summary>
        /// <param name="status">Status do recurso (ex: Available, InUse)</param>
        /// <returns>Lista de recursos com o status especificado</returns>
        /// <response code="200">Retorna a lista de recursos com o status especificado</response>
        /// <response code="400">Status do recurso inválido</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(IEnumerable<Resource>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Resource>>> GetResourcesByStatus(ResourceStatus status)
        {
            try
            {
                IEnumerable<Resource> resources = await _resourceService.GetResourcesByStatusAsync(status);
                return Ok(resources);
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
        /// Obtém recursos dentro de um raio específico de uma localização
        /// </summary>
        /// <param name="latitude">Latitude da localização central</param>
        /// <param name="longitude">Longitude da localização central</param>
        /// <param name="radiusKm">Raio de busca em quilômetros (padrão: 5km, máximo: 100km)</param>
        /// <returns>Lista de recursos dentro do raio especificado</returns>
        /// <response code="200">Retorna a lista de recursos na área especificada</response>
        /// <response code="400">Parâmetros de localização ou raio inválidos</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("nearby")]
        [ProducesResponseType(typeof(IEnumerable<Resource>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Resource>>> GetNearbyResources(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] double radiusKm = 5.0)
        {
            try
            {
                if (latitude < -90 || latitude > 90)
                    return BadRequest("A latitude deve estar entre -90 e 90 graus.");
                if (longitude < -180 || longitude > 180)
                    return BadRequest("A longitude deve estar entre -180 e 180 graus.");
                if (radiusKm <= 0 || radiusKm > 100)
                    return BadRequest("O raio deve estar entre 0 e 100 km.");

                IEnumerable<Resource> resources = await _resourceService.GetNearbyResourcesAsync(latitude, longitude, radiusKm);
                return Ok(resources);
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
        /// Cria um novo recurso no sistema
        /// </summary>
        /// <param name="resource">Dados do recurso a ser criado</param>
        /// <returns>Dados do recurso criado</returns>
        /// <response code="201">Recurso criado com sucesso</response>
        /// <response code="400">Dados do recurso inválidos</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(Resource), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Administrator, Authority, Volunteer")]
        public async Task<ActionResult<Resource>> PostResource([FromBody] Resource resource)
        {
            if (resource == null)
                return BadRequest("Dados do recurso não podem ser nulos.");

            try
            {
                Resource createdResource = await _resourceService.CreateResourceAsync(resource);
                return CreatedAtAction(nameof(GetResource), new { id = createdResource.Id }, createdResource);
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
        /// Atualiza um recurso existente
        /// </summary>
        /// <param name="id">ID do recurso a ser atualizado</param>
        /// <param name="resource">Dados atualizados do recurso</param>
        /// <returns>Confirmação da atualização</returns>
        /// <response code="204">Recurso atualizado com sucesso</response>
        /// <response code="400">Dados inválidos ou ID não corresponde</response>
        /// <response code="404">Recurso não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Administrator, Authority, Volunteer")]
        public async Task<IActionResult> PutResource(long id, [FromBody] Resource resource)
        {
            if (resource == null)
                return BadRequest("Dados do recurso não podem ser nulos.");

            try
            {
                Resource updatedResource = await _resourceService.UpdateResourceAsync(id, resource);
                return updatedResource != null ? NoContent() : NotFound($"Recurso com ID {id} não foi encontrado");
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
        /// Remove um recurso do sistema
        /// </summary>
        /// <param name="id">ID do recurso a ser removido</param>
        /// <returns>Confirmação da remoção</returns>
        /// <response code="204">Recurso removido com sucesso</response>
        /// <response code="400">ID inválido</response>
        /// <response code="404">Recurso não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteResource(long id)
        {
            try
            {
                bool deleted = await _resourceService.DeleteResourceAsync(id);
                return deleted ? NoContent() : NotFound($"Recurso com ID {id} não foi encontrado");
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