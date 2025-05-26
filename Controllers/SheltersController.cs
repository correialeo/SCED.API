using Microsoft.AspNetCore.Mvc;
using SCED.API.Domain.Entity;
using SCED.API.DTO;
using SCED.API.Interfaces;

namespace SCED.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SheltersController : ControllerBase
    {
        private readonly IShelterService _shelterService;

        public SheltersController(IShelterService shelterService)
        {
            _shelterService = shelterService;
        }

        // GET: api/Shelters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Shelter>>> GetShelters()
        {
            var shelters = await _shelterService.GetAllSheltersAsync();
            return Ok(shelters);
        }

        // GET: api/Shelters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Shelter>> GetShelter(long id)
        {
            var shelter = await _shelterService.GetShelterByIdAsync(id);

            if (shelter == null)
            {
                return NotFound();
            }

            return Ok(shelter);
        }

        // GET: api/Shelters/nearby?latitude=-23.5505&longitude=-46.6333&radius=10
        [HttpGet("nearby")]
        public async Task<ActionResult<IEnumerable<Shelter>>> GetNearbyShelters(
            [FromQuery] double latitude, 
            [FromQuery] double longitude, 
            [FromQuery] double radius = 10.0)
        {
            if (latitude < -90 || latitude > 90)
                return BadRequest("Latitude deve estar entre -90 e 90");

            if (longitude < -180 || longitude > 180)
                return BadRequest("Longitude deve estar entre -180 e 180");

            if (radius <= 0 || radius > 100)
                return BadRequest("Raio deve estar entre 0 e 100 km");

            var nearbyShelters = await _shelterService.GetNearbySheltersAsync(latitude, longitude, radius);
            return Ok(nearbyShelters);
        }

        // GET: api/Shelters/available
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<Shelter>>> GetAvailableShelters()
        {
            var availableShelters = await _shelterService.GetAvailableSheltersAsync();
            return Ok(availableShelters);
        }

        // PUT: api/Shelters/5 - Melhor abordagem para PUT
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShelter(long id, Shelter shelter)
        {
            try
            {
                var updatedShelter = await _shelterService.UpdateShelterAsync(id, shelter);
                
                if (updatedShelter == null)
                {
                    return NotFound();
                }

                return Ok(updatedShelter);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar abrigo: {ex.Message}");
            }
        }

        // PATCH: api/Shelters/5/capacity - Endpoint específico para atualizar ocupação
        [HttpPatch("{id}/capacity")]
        public async Task<IActionResult> UpdateShelterCapacity(long id, [FromBody] UpdateCapacityRequestDTO request)
        {
            try
            {
                var updated = await _shelterService.UpdateCapacityAsync(id, request.CurrentOccupancy);
                
                if (!updated)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/Shelters
        [HttpPost]
        public async Task<ActionResult<Shelter>> PostShelter(Shelter shelter)
        {
            try
            {
                var createdShelter = await _shelterService.CreateShelterAsync(shelter);
                return CreatedAtAction("GetShelter", new { id = createdShelter.Id }, createdShelter);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar abrigo: {ex.Message}");
            }
        }

        // DELETE: api/Shelters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShelter(long id)
        {
            var deleted = await _shelterService.DeleteShelterAsync(id);
            
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}