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

        public SensorController(SensorDataRepository sensorDataRepository)
        {
            _sensorDataRepository = sensorDataRepository;
        }   

        [HttpPost("add")]
        public async Task<IActionResult> AddSensorData([FromBody] SensorData data)
        {
           try
            {
                await _sensorDataRepository.AddSensorDataAsync(data);
                return Ok(new { message = "Sensor data inserted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
            
        }
    }
}