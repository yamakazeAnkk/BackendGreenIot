using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using GreenIotApi.Services;
using GreenIotApi.DTOs;
using GreenIotApi.Models;

namespace GreenIotApi.Controllers
{
    [ApiController]
    [Route("api/devices")]
    public class DeviceController : ControllerBase
    {
        private readonly FirestoreService _firestoreService;
        private readonly IMapper _mapper;

        public DeviceController(FirestoreService firestoreService, IMapper mapper)
        {
            _firestoreService = firestoreService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> AddDevice(string gardenId, [FromBody] DeviceDto deviceDto)
        {
            if(string.IsNullOrEmpty(deviceDto.Name))
            {
                return BadRequest("Device name is required.");
            }
            var existingDevices = await _firestoreService.GetDevicesAsync(gardenId);
            if (existingDevices.Any(d => d.Name.Equals(deviceDto.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return Conflict(new { Message = "A device with the same name already exists in this garden." });
            }
            if(!await _firestoreService.CheckGardenExistsAsync(gardenId))
            {
                return NotFound("GardenId does not exist.");
            }
            var device = _mapper.Map<Device>(deviceDto);
            var deviceId = await _firestoreService.AddDeviceAsync(gardenId, device);
            return Ok(new { DeviceId = deviceId, Message = "Device added successfully." });
        }

        // Get All Devices from a Garden
        [HttpGet]
        public async Task<IActionResult> GetDevices(string gardenId)
        {
            var devices = await _firestoreService.GetDevicesAsync(gardenId);
            if (!devices.Any())
                return NotFound("No devices found.");
            return Ok(devices);
        }
    }
}
