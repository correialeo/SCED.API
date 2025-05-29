using Microsoft.AspNetCore.Mvc;
using SCED.API.Domain.Interfaces;
using SCED.API.Domain.Enums;
using SCED.API.DTO;
using Microsoft.AspNetCore.Authorization;

namespace SCED.API.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de usuários do sistema SCED
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Obtém todos os usuários do sistema
        /// </summary>
        /// <returns>Lista de usuários</returns>
        /// <response code="200">Lista de usuários retornada com sucesso</response>
        /// <response code="401">Token de autenticação inválido ou ausente</response>
        /// <response code="403">Usuário não possui permissão para acessar este recurso</response>
        [HttpGet]
        [Authorize(Roles = "Administrator,Authority")]
        [ProducesResponseType(typeof(IEnumerable<UserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAll()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                var userDtos = users.Select(u => new UserDTO
                {
                    Id = u.Id,
                    Username = u.Username,
                    Role = u.Role,
                    Necessities = u.Necessities,
                    Latitude = u.Latitude,
                    Longitude = u.Longitude
                });

                return Ok(userDtos);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Obtém um usuário específico por ID
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <returns>Dados do usuário</returns>
        /// <response code="200">Usuário encontrado e retornado com sucesso</response>
        /// <response code="401">Token de autenticação inválido ou ausente</response>
        /// <response code="403">Usuário não possui permissão para acessar este recurso</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDTO>> GetById([FromRoute] long id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "ID do usuário deve ser maior que zero" });
                }

                var currentUserId = GetCurrentUserId();
                var currentUserRole = GetCurrentUserRole();

                // Usuários só podem ver seus próprios dados, exceto administradores
                if (currentUserId != id && currentUserRole != UserRole.Administrator && currentUserRole != UserRole.Authority)
                {
                    return Forbid();
                }

                var user = await _userRepository.GetByIdAsync(id);
                if (user == null) 
                {
                    return NotFound(new { message = "Usuário não encontrado" });
                }

                var userDto = new UserDTO
                {
                    Id = user.Id,
                    Username = user.Username,
                    Role = user.Role,
                    Necessities = user.Necessities,
                    Latitude = user.Latitude,
                    Longitude = user.Longitude
                };

                return Ok(userDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Remove um usuário do sistema
        /// </summary>
        /// <param name="id">ID do usuário a ser removido</param>
        /// <returns>Status da operação</returns>
        /// <response code="204">Usuário removido com sucesso</response>
        /// <response code="401">Token de autenticação inválido ou ausente</response>
        /// <response code="403">Usuário não possui permissão para realizar esta operação</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpDelete("{id:long}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete([FromRoute] long id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "ID do usuário deve ser maior que zero" });
                }

                var deleted = await _userRepository.DeleteAsync(id);
                return deleted ? NoContent() : NotFound(new { message = "Usuário não encontrado" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Erro interno do servidor" });
            }
        }

        private long GetCurrentUserId()
        {
            // Corrigido: usar "UserId" claim personalizado em vez de NameIdentifier
            var userIdClaim = User.FindFirst("UserId")?.Value;
            return long.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        private UserRole GetCurrentUserRole()
        {
            var roleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            return Enum.TryParse<UserRole>(roleClaim, out var role) ? role : UserRole.Victim;
        }
    }
}