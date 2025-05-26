using Microsoft.AspNetCore.Mvc;
using SCED.API.Domain.Entity;
using SCED.API.Interfaces;

namespace SCED.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourcesController : ControllerBase
    {
        private readonly IResourceService _resourceService;

        public ResourcesController(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }

        // GET: api/Resources
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Resource>>> GetResources()
        {
            IEnumerable<Resource>? resources = await _resourceService.GetAllResourcesAsync();
            return Ok(resources);
        }

        // GET: api/Resources/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Resource>> GetResource(long id)
        {
            Resource? resource = await _resourceService.GetResourceByIdAsync(id);

            if (resource == null)
            {
                return NotFound();
            }

            return Ok(resource);
        }

        // GET: api/Resources/nearby?latitude=-23.5505&longitude=-46.6333&radius=10
        [HttpGet("nearby")]
        public async Task<ActionResult<IEnumerable<Resource>>> GetNearbyResources(
            [FromQuery] double latitude, 
            [FromQuery] double longitude, 
            [FromQuery] double radius = 5.0)
        {
            if (latitude < -90 || latitude > 90)
                return BadRequest("Latitude deve estar entre -90 e 90");

            if (longitude < -180 || longitude > 180)
                return BadRequest("Longitude deve estar entre -180 e 180");

            if (radius <= 0 || radius > 100)
                return BadRequest("Raio deve estar entre 0 e 100 km");

            var nearbyResources = await _resourceService.GetNearbyResourcesAsync(latitude, longitude, radius);
            return Ok(nearbyResources);
        }

        // PUT: api/Resources/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutResource(long id, Resource resource)
        {
            if (id != resource.Id)
            {
                return BadRequest("ID do recurso não confere");
            }

            try
            {
                await _resourceService.UpdateResourceAsync(id, resource);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar recurso: {ex.Message}");
            }
        }

        // POST: api/Resources
        [HttpPost]
        public async Task<ActionResult<Resource>> PostResource(Resource resource)
        {
            try
            {
                Resource? createdResource = await _resourceService.CreateResourceAsync(resource);
                return CreatedAtAction("GetResource", new { id = createdResource.Id }, createdResource);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar recurso: {ex.Message}");
            }
        }

        // DELETE: api/Resources/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResource(long id)
        {
            bool deleted = await _resourceService.DeleteResourceAsync(id);
            
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}