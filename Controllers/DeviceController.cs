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
        public async Task<IActionResult> AddDevice([FromBody] DeviceDto deviceDto, string gardenId = "6jk2PWTaobJgQCGPlVso")
        {
            if (string.IsNullOrEmpty(deviceDto.Name))
            {
                return BadRequest("Device name is required.");
            }

            // Tách các thiết bị từ chuỗi đầu vào
            var deviceNames = deviceDto.Name.Split(',').Select(name => name.Trim()).ToList();

            // Kiểm tra các thiết bị đã có trong cơ sở dữ liệu
            var existingDevices = await _deviceService.GetDevicesAsync(gardenId);

            var successfullyAddedDevices = new List<string>();
            var failedDevices = new List<string>();

            foreach (var deviceName in deviceNames)
            {
                // Kiểm tra xem thiết bị đã tồn tại chưa
                if (existingDevices.Any(d => d.Name.Equals(deviceName, StringComparison.OrdinalIgnoreCase)))
                {
                    failedDevices.Add(deviceName); // Nếu thiết bị đã có, lưu vào danh sách thất bại
                    continue;
                }

                // Kiểm tra xem gardenId có tồn tại không
                if (!await _deviceService.CheckGardenExistsAsync(gardenId))
                {
                    return NotFound("GardenId does not exist.");
                }

                // Thêm thiết bị vào Firestore
                var device = new Device { Name = deviceName }; // Chỉ lấy tên thiết bị từ input
                try
                {
                    var deviceId = await _deviceService.AddDeviceAsync(gardenId, device);
                    successfullyAddedDevices.Add(deviceName); // Thêm vào danh sách thành công
                }
                catch (Exception ex)
                {
                    failedDevices.Add(deviceName); // Nếu có lỗi (thiết bị trùng tên), lưu vào danh sách thất bại
                }
            }

            // Trả về thông tin thiết bị đã được thêm và các thiết bị bị thất bại
            return Ok(new
            {
                Success = successfullyAddedDevices,
                Failed = failedDevices
            });
        }


        // Get All Devices from a Garden
        [HttpGet]
        public async Task<IActionResult> GetDevices(string gardenId = "6jk2PWTaobJgQCGPlVso")
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
                return Ok(new { Message = "Data updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
        
    }
}
