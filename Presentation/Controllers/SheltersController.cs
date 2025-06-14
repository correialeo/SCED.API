﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCED.API.Application.Interfaces;
using SCED.API.Domain.Entity;
using SCED.API.Presentation.DTO;

namespace SCED.API.Presentation.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de abrigos do sistema SCED
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize]
    public class SheltersController : ControllerBase
    {
        private readonly IShelterService _shelterService;

        /// <summary>
        /// Construtor do SheltersController
        /// </summary>
        /// <param name="shelterService">Serviço de abrigos</param>
        public SheltersController(IShelterService shelterService)
        {
            _shelterService = shelterService ?? throw new ArgumentNullException(nameof(shelterService));
        }

        /// <summary>
        /// Obtém todos os abrigos cadastrados no sistema
        /// </summary>
        /// <returns>Lista de todos os abrigos</returns>
        /// <response code="200">Retorna a lista de abrigos</response>
        /// <response code="401">Token de autenticação inválido ou ausente</response>
        /// <response code="403">Usuário não possui permissão para acessar este recurso</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Shelter>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Shelter>>> GetShelters()
        {
            try
            {
                IEnumerable<Shelter> shelters = await _shelterService.GetAllSheltersAsync();
                return Ok(shelters);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém um abrigo específico pelo seu ID
        /// </summary>
        /// <param name="id">ID do abrigo</param>
        /// <returns>Dados do abrigo solicitado</returns>
        /// <response code="200">Retorna o abrigo encontrado</response>
        /// <response code="400">ID inválido</response>
        /// <response code="401">Token de autenticação inválido ou ausente</response>
        /// <response code="403">Usuário não possui permissão para acessar este recurso</response>
        /// <response code="404">Abrigo não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(Shelter), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Shelter>> GetShelter(long id)
        {
            try
            {
                Shelter shelter = await _shelterService.GetShelterByIdAsync(id);
                return shelter != null ? Ok(shelter) : NotFound($"Abrigo com ID {id} não foi encontrado");
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
        /// Obtém abrigos dentro de um raio específico de uma localização
        /// </summary>
        /// <param name="latitude">Latitude da localização central</param>
        /// <param name="longitude">Longitude da localização central</param>
        /// <param name="radiusKm">Raio de busca em quilômetros (padrão: 10km, máximo: 1000km)</param>
        /// <returns>Lista de abrigos dentro do raio especificado</returns>
        /// <response code="200">Retorna a lista de abrigos na área especificada</response>
        /// <response code="400">Parâmetros de localização ou raio inválidos</response>
        /// <response code="401">Token de autenticação inválido ou ausente</response>
        /// <response code="403">Usuário não possui permissão para acessar este recurso</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("nearby")]
        [ProducesResponseType(typeof(IEnumerable<Shelter>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Shelter>>> GetNearbyShelters(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] double radiusKm = 10.0)
        {
            try
            {
                if (latitude < -90 || latitude > 90)
                    return BadRequest("A latitude deve estar entre -90 e 90 graus.");
                if (longitude < -180 || longitude > 180)
                    return BadRequest("A longitude deve estar entre -180 e 180 graus.");
                if (radiusKm <= 0 || radiusKm > 1000)
                    return BadRequest("O raio deve estar entre 0 e 1000 km.");

                IEnumerable<Shelter> shelters = await _shelterService.GetNearbySheltersAsync(latitude, longitude, radiusKm);
                return Ok(shelters);
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
        /// Cria um novo abrigo no sistema
        /// </summary>
        /// <param name="shelter">Dados do abrigo a ser criado</param>
        /// <returns>Dados do abrigo criado</returns>
        /// <response code="201">Abrigo criado com sucesso</response>
        /// <response code="400">Dados do abrigo inválidos</response>
        /// <response code="401">Token de autenticação inválido ou ausente</response>
        /// <response code="403">Usuário não possui permissão para acessar este recurso</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(Shelter), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Authority, Administrator")]
        public async Task<ActionResult<Shelter>> PostShelter([FromBody] Shelter shelter)
        {
            if (shelter == null)
                return BadRequest("Dados do abrigo não podem ser nulos.");

            try
            {
                Shelter createdShelter = await _shelterService.CreateShelterAsync(shelter);
                return CreatedAtAction(nameof(GetShelter), new { id = createdShelter.Id }, createdShelter);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno inesperado: {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza um abrigo existente
        /// </summary>
        /// <param name="id">ID do abrigo a ser atualizado</param>
        /// <param name="shelter">Dados atualizados do abrigo</param>
        /// <returns>Confirmação da atualização</returns>
        /// <response code="204">Abrigo atualizado com sucesso</response>
        /// <response code="400">Dados inválidos ou ID não corresponde</response>
        /// <response code="401">Token de autenticação inválido ou ausente</response>
        /// <response code="403">Usuário não possui permissão para acessar este recurso</response>
        /// <response code="404">Abrigo não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Roles = "Authority, Administrator")]
        public async Task<IActionResult> PutShelter(long id, [FromBody] Shelter shelter)
        {
            if (shelter == null)
                return BadRequest("Dados do abrigo não podem ser nulos.");

            try
            {
                Shelter updatedShelter = await _shelterService.UpdateShelterAsync(id, shelter);
                return updatedShelter != null ? NoContent() : NotFound($"Abrigo com ID {id} não foi encontrado");
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
        /// Atualiza a ocupação atual de um abrigo
        /// </summary>
        /// <param name="id">ID do abrigo</param>
        /// <param name="request">Objeto contendo o novo valor de ocupação</param>
        /// <returns>Confirmação da atualização</returns>
        /// <response code="204">Ocupação atualizada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="401">Token de autenticação inválido ou ausente</response>
        /// <response code="403">Usuário não possui permissão para acessar este recurso</response>
        /// <response code="404">Abrigo não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPatch("{id:long}/capacity")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize (Roles = "Authority, Administrator")]
        public async Task<IActionResult> UpdateShelterCapacity(long id, [FromBody] UpdateCapacityRequestDTO request)
        {
            if (request == null)
                return BadRequest("Dados da atualização não podem ser nulos.");

            try
            {
                bool updated = await _shelterService.UpdateCapacityAsync(id, request.CurrentOccupancy);
                return updated ? NoContent() : NotFound($"Abrigo com ID {id} não foi encontrado");
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
        /// Remove um abrigo do sistema
        /// </summary>
        /// <param name="id">ID do abrigo a ser removido</param>
        /// <returns>Confirmação da remoção</returns>
        /// <response code="204">Abrigo removido com sucesso</response>
        /// <response code="400">ID inválido</response>
        /// <response code="401">Token de autenticação inválido ou ausente</response>
        /// <response code="403">Usuário não possui permissão para acessar este recurso</response>
        /// <response code="404">Abrigo não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize (Roles = "Authority, Administrator")]
        public async Task<IActionResult> DeleteShelter(long id)
        {
            try
            {
                bool deleted = await _shelterService.DeleteShelterAsync(id);
                return deleted ? NoContent() : NotFound($"Abrigo com ID {id} não foi encontrado");
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