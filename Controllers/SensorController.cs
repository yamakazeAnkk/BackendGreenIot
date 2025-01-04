using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenIotApi.Models;
using GreenIotApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GreenIotApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorController : ControllerBase
    {
        private readonly SensorDataRepository _sensorDataRepository;
        private readonly ILogger<SensorController> _logger;

        public SensorController(SensorDataRepository sensorDataRepository, ILogger<SensorController> logger)
        {
            _sensorDataRepository = sensorDataRepository;
            _logger = logger;
        }   

        [HttpPost("add")]
        public async Task<IActionResult> AddSensorData([FromBody] SensorData data)
        {
           try
            {
                _logger.LogInformation("Received SensorData: {@Data}", data);
                await _sensorDataRepository.AddSensorDataAsync(data);
                return Ok(new { message = "Sensor data inserted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting SensorData");
                return StatusCode(500, new { error = ex.Message });
            }
            
        }
    }
}