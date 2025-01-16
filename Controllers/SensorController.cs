using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using GreenIotApi.Services;
using GreenIotApi.DTOs;
using GreenIotApi.Models;

namespace GreenIotApi.Controllers
{
    [ApiController]
    [Route("api/sensor-data")]
    public class SensorDataController : ControllerBase
    {
        private readonly FirestoreService _firestoreService;
        private readonly IMapper _mapper;

        public SensorDataController(FirestoreService firestoreService, IMapper mapper)
        {
            _firestoreService = firestoreService;
            _mapper = mapper;
        }

        // POST: Add sensor data to a garden
        [HttpPost]
        public async Task<IActionResult> AddSensorData(string gardenId, [FromBody] SensorDataDto sensorDataDto)
        {
            var sensorData = _mapper.Map<SensorData>(sensorDataDto);
            var sensorDataId = await _firestoreService.AddSensorDataAsync(gardenId, sensorData);
            return Ok(new { SensorDataId = sensorDataId, Message = "Sensor data added successfully." });
        }

        // Get All SensorData from a Garden
        [HttpGet]
        public async Task<IActionResult> GetSensorData(string gardenId)
        {
            var data = await _firestoreService.GetSensorDataAsync(gardenId);
            if (!data.Any())
                return NotFound("No sensor data found.");
            return Ok(data);
        }

    }
}
