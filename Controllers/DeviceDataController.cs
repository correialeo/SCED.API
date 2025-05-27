using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SCED.API.Domain.Entity;
using SCED.API.Services;
using SCED.API.DTO;

namespace SCED.API.Controllers
{
    [ApiController]
    [Route("api/data")]
    public class DeviceDataController : ControllerBase
    {
        private readonly DeviceDataService _deviceDataService;
        public DeviceDataController(DeviceDataService deviceDataService)
        {
            _deviceDataService = deviceDataService ?? throw new ArgumentNullException(nameof(deviceDataService));
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveData([FromBody] DeviceDataDTO deviceData)
        {
            if (deviceData == null)
            {
                return BadRequest("Device data cannot be null");
            }

            var response = await _deviceDataService.ReceiveDataAsync(deviceData);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Data);
        }

        
    }
}