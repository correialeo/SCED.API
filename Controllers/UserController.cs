using Microsoft.AspNetCore.Mvc;
using SCED.API.Domain.Interfaces;
using SCED.API.Domain.Enums;
using SCED.API.DTO;
using Microsoft.AspNetCore.Authorization;

namespace SCED.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Administrator, Authority")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAll()
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

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetById(long id)
        {
            var currentUserId = GetCurrentUserId();
            var currentUserRole = GetCurrentUserRole();

            if (currentUserId != id && currentUserRole != UserRole.Administrator)
            {
                return Forbid();
            }

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();

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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Delete(long id)
        {
            var deleted = await _userRepository.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }

        private long GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return long.TryParse(userIdClaim, out var userId) ? userId : 0;
        }

        private UserRole GetCurrentUserRole()
        {
            var roleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            return Enum.TryParse<UserRole>(roleClaim, out var role) ? role : UserRole.Victim;
        }
    }
}