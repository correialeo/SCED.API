using Microsoft.AspNetCore.Mvc;
using SCED.API.Domain.Entity;
using SCED.API.DTO;
using SCED.API.Interfaces;

namespace SCED.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly IDeviceService _deviceService;

        public DevicesController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Device>>> GetDevices()
        {
            var devices = await _deviceService.GetAllDevicesAsync();
            return Ok(devices);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Device>> GetDevice(long id)
        {
            var device = await _deviceService.GetDeviceByIdAsync(id);
            return device != null ? Ok(device) : NotFound();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDevice(long id, Device device)
        {
            if (id != device.Id)
                return BadRequest("ID do dispositivo não confere");

            try
            {
                var updatedDevice = await _deviceService.UpdateDeviceAsync(id, device);
                return updatedDevice != null ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar dispositivo: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Device>> PostDevice(DeviceDTO deviceDTO)
        {
            if (deviceDTO == null)
                return BadRequest("Dados do dispositivo inválidos");

            var device = new Device
            {
                Type = deviceDTO.Type,
                Status = deviceDTO.Status,
                Latitude = deviceDTO.Latitude,
                Longitude = deviceDTO.Longitude
            };

            try
            {
                var createdDevice = await _deviceService.CreateDeviceAsync(device);
                return CreatedAtAction("GetDevice", new { id = createdDevice.Id }, createdDevice);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar dispositivo: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDevice(long id)
        {
            var deleted = await _deviceService.DeleteDeviceAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}