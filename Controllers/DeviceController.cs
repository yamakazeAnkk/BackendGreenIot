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
        private readonly DeviceService _deviceService;
        private readonly IMapper _mapper;
        private readonly FirebaseStorageService _firebaseStorageService;

        public DeviceController(DeviceService deviceService, IMapper mapper, FirebaseStorageService firebaseStorageService)
        {
            _deviceService = deviceService;
            _mapper = mapper;
            _firebaseStorageService = firebaseStorageService;
        }
        [HttpGet("getData/{nodeId}")]
        public async Task<IActionResult> GetData(string nodeId)
        {
            try
            {
                var data = await _firebaseStorageService.GetDataFromRealtimeDatabaseAsync(nodeId);
                return Ok(data); // Return the fetched data
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddDevice(string gardenId, [FromBody] DeviceDto deviceDto)
        {
            if(string.IsNullOrEmpty(deviceDto.Name))
            {
                return BadRequest("Device name is required.");
            }
            var existingDevices = await _deviceService.GetDevicesAsync(gardenId);
            if (existingDevices.Any(d => d.Name.Equals(deviceDto.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return Conflict(new { Message = "A device with the same name already exists in this garden." });
            }
            if(!await _deviceService.CheckGardenExistsAsync(gardenId))
            {
                return NotFound("GardenId does not exist.");
            }
            var device = _mapper.Map<Device>(deviceDto);
            var deviceId = await _deviceService.AddDeviceAsync(gardenId, device);
            return Ok(new { DeviceId = deviceId, Message = "Device added successfully." });
        }

        // Get All Devices from a Garden
        [HttpGet]
        public async Task<IActionResult> GetDevices(string gardenId)
        {
            var devices = await _deviceService.GetDevicesAsync(gardenId);
            if (!devices.Any())
                return NotFound("No devices found.");
            return Ok(devices);
        }
        [HttpPatch("updateData/{nodeId}")]
        public async Task<IActionResult> UpdateData(string nodeId, [FromBody] FirebaseData data)
        {
            try
            {
                await _firebaseStorageService.UpdateDataToRealtimeDatabaseAsync(nodeId, data);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
        
    }
}
