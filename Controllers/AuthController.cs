using Microsoft.AspNetCore.Mvc;
using SCED.API.DTO;
using SCED.API.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SCED.API.Controllers
{
    /// <summary>
    /// Controller responsável pela autenticação e autorização de usuários
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Realiza o login de um usuário no sistema
        /// </summary>
        /// <param name="request">Dados de login (username e password)</param>
        /// <returns>Token JWT e informações do usuário</returns>
        /// <response code="200">Login realizado com sucesso</response>
        /// <response code="400">Dados de entrada inválidos</response>
        /// <response code="401">Credenciais inválidas</response>
        /// <response code="429">Muitas tentativas de login - tente novamente mais tarde</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(object), StatusCodes.Status429TooManyRequests)]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { 
                        message = "Dados de entrada inválidos", 
                        errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                var response = await _authService.LoginAsync(request);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Registra um novo usuário no sistema
        /// </summary>
        /// <param name="request">Dados do novo usuário</param>
        /// <returns>Dados do usuário criado</returns>
        /// <response code="201">Usuário criado com sucesso</response>
        /// <response code="400">Dados de entrada inválidos ou usuário já existe</response>
        /// <response code="422">Erro de validação dos dados</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<UserDTO>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { 
                        message = "Dados de entrada inválidos", 
                        errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                var user = await _authService.RegisterAsync(request);
                return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return UnprocessableEntity(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Erro interno do servidor" });
            }
        }

        /// <summary>
        /// Valida se um token JWT é válido
        /// </summary>
        /// <param name="tokenRequest">Token a ser validado</param>
        /// <returns>Status de validade do token</returns>
        /// <response code="200">Token válido</response>
        /// <response code="400">Token não fornecido ou formato inválido</response>
        /// <response code="401">Token inválido ou expirado</response>
        [HttpPost("validate")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> ValidateToken([FromBody] TokenValidationRequest tokenRequest)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tokenRequest?.Token))
                {
                    return BadRequest(new { message = "Token é obrigatório", valid = false });
                }

                var isValid = await _authService.ValidateTokenAsync(tokenRequest.Token);
                
                if (isValid)
                {
                    var userInfo = await _authService.GetUserFromTokenAsync(tokenRequest.Token);
                    return Ok(new { valid = true, user = userInfo });
                }
                
                return Unauthorized(new { valid = false, message = "Token inválido ou expirado" });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Erro interno do servidor", valid = false });
            }
        }

        /// <summary>
        /// Realiza logout do usuário (invalidação do token - se implementado)
        /// </summary>
        /// <returns>Status da operação</returns>
        /// <response code="200">Logout realizado com sucesso</response>
        /// <response code="401">Token inválido</response>
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Logout()
        {
            // Se você implementar blacklist de tokens, aqui seria o lugar
            return Ok(new { message = "Logout realizado com sucesso" });
        }
    }

    /// <summary>
    /// Request para validação de token
    /// </summary>
    public class TokenValidationRequest
    {
        /// <summary>
        /// Token JWT a ser validado
        /// </summary>
        [Required(ErrorMessage = "Token é obrigatório")]
        public string Token { get; set; } = string.Empty;
    }
}