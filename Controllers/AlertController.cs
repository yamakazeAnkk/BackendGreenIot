using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GreenIotApi.Services;
using GreenIotApi.Repositories;
using GreenIotApi.Models;

namespace GreenIotApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlertController : ControllerBase
    {
       

        private readonly SensorDataService _sensorDataService;
        private readonly GardenService _gardenService;
        



        public AlertController(SensorDataService sensorDataService, GardenService gardenService)
        {
            _sensorDataService = sensorDataService;
            _gardenService = gardenService;
  
        }   
        [HttpPost("check-sensors")]
        public async Task<IActionResult> CheckSensorsForUser([FromBody] CheckSensorRequest request)
        {
            try
            {
                // Gọi service để kiểm tra tất cả các cảm biến của người dùng
                // Bạn có thể thực hiện các thao tác logic xử lý ở đây
                await _sensorDataService.CheckSensorsForUserAsync(request.UserId, request.Date);

                return Ok(new { Message = "Sensor check completed successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Error: {ex.Message}" });
            }
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAlertsByUserIdAsync(string userId)
        {
            try
            {
                // Get the alerts for the user
                var alerts = await _sensorDataService.GetAlertsByUserIdAsync(userId);

                // Get garden names for each alert
                var alertsWithGardenNames = new List<object>();

                foreach (var alert in alerts)
                {
                    var garden = await _gardenService.GetGardenAsync(alert.GardenId);
                    if (garden != null)
                    {
                        alertsWithGardenNames.Add(new
                        {
                            AlertId = alert.AlertId,
                            Message = alert.Message,
                            GardenName = garden.Name, // Assuming Garden has a Name property
                            Timestamp = alert.Timestamp,
                            Resolved = alert.Resolved
                        });
                    }
                }

                return Ok(alertsWithGardenNames);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Error: {ex.Message}" });
            }
        }
        
        
    }
}