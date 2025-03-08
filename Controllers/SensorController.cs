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
        private readonly SensorDataService _sensorDataService;
        private readonly IMapper _mapper;

        public SensorDataController(SensorDataService sensorDataService, IMapper mapper)
        {
            _sensorDataService = sensorDataService;
            _mapper = mapper;
        }

        // POST: Add sensor data to a garden
        [HttpPost]
        public async Task<IActionResult> AddSensorData(string gardenId, [FromBody] SensorDataDto sensorDataDto)
        {
            var sensorData = _mapper.Map<SensorData>(sensorDataDto);
            var sensorDataId = await _sensorDataService.AddSensorDataAsync(gardenId, sensorData);
            return Ok(new { SensorDataId = sensorDataId, Message = "Sensor data added successfully." });
        }

        // Get All SensorData from a Garden
        

        [HttpGet("{gardenId}/latest")]
        public async Task<IActionResult> GetLatestSensorData(string gardenId)
        {
            var data = await _sensorDataService.GetLatestSensorDataAsync(gardenId);
            return Ok(data);
        }

        [HttpGet("{gardenId}/daily")]
        public async Task<IActionResult> GetDailySensorData(string gardenId, DateTime? date = null)
        {
            date ??= DateTime.UtcNow.AddDays(-(int)DateTime.UtcNow.DayOfWeek);
            var data = await _sensorDataService.GetDailySensorDataAsync(gardenId, date.Value);
            return Ok(data);
        }

        [HttpGet("{gardenId}/weekly")]
        public async Task<IActionResult> GetWeeklySensorData(string gardenId, DateTime? weekStart = null )
        {
            weekStart ??= DateTime.UtcNow.AddDays(-(int)DateTime.UtcNow.DayOfWeek);
            var data = await _sensorDataService.GetWeeklySensorDataAsync(gardenId, weekStart.Value);
            return Ok(data);
        }
        [HttpGet("{gardenId}/monthly")]
        public async Task<IActionResult> GetMonthlySensorData(string gardenId, DateTime? monthStart = null  )
        {
            monthStart ??= DateTime.UtcNow.AddMonths(-(int)DateTime.UtcNow.Month);
            var data = await _sensorDataService.GetMonthlySensorDataAsync(gardenId, monthStart.Value);
            return Ok(data);
        }   
        [HttpGet("{gardenId}/yearly")]
        public async Task<IActionResult> GetYearlySensorData(string gardenId, int year = 2025, int month = 1)
        {
            var data = await _sensorDataService.GetYearlySensorDataAsync(gardenId, year, month);
            return Ok(data);
        }
        [HttpGet("time/{nodeId}")]
        public async Task<IActionResult> GetSoilMoistureTimeSeries(string nodeId = "6jk2PWTaobJgQCGPlVso", 
                                                                  [FromQuery] int year = 2025, 
                                                                  [FromQuery] int month = 2, 
                                                                  [FromQuery] int? day = 1,
                                                                  [FromQuery] string columnName = "SoilMoisture")     
        {
            try
            {
                // Fetch the time-series data for SoilMoisture
                var timeSeriesData = await _sensorDataService.GetTimeSeriesDataAsync(nodeId, year, month, day , columnName);
                
                // Return the SoilMoisture data
                return Ok(new { data = timeSeriesData[columnName] }); // Return the array of SoilMoisture values
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
        [HttpGet("monthlyAverage/{nodeId}")]
        public async Task<IActionResult> GetMonthlyAverageByWeek(string nodeId, 
                                                            [FromQuery] int year, 
                                                            [FromQuery] int month,
                                                            [FromQuery] string columnName = "SoilMoisture") // Default is SoilMoisture
        {
            try
            {
                // Lấy dữ liệu trung bình của từng tuần trong tháng cho cột chỉ định
                var weeklyAverages = await _sensorDataService.GetMonthlyAverageByWeekAsync(nodeId, year, month, columnName);

                return Ok(new { data = weeklyAverages }); // Trả về mảng trung bình cho 4 tuần
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
        [HttpGet("weeklyAverage/{nodeId}")]
        public async Task<IActionResult> GetWeeklyDataByColumn(string nodeId = "6jk2PWTaobJgQCGPlVso", 
                                                            [FromQuery] int year = 2025, 
                                                            [FromQuery] int month = 2,
                                                            [FromQuery] int day = 1,
                                                            [FromQuery] string columnName = "SoilMoisture") // Default is SoilMoisture
        {
            try
            {
                // Lấy dữ liệu trung bình của từng ngày trong tuần cho cột chỉ định
                var weeklyData = await _sensorDataService.GetWeeklyDataByColumnAsync(nodeId, year, month, day, columnName);

                return Ok(new { data = weeklyData }); // Trả về mảng trung bình cho 7 ngày
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
        [HttpPost("check-sensors")]
        public async Task<IActionResult> CheckSensorsForUser([FromBody] CheckSensorRequest request)
        {
            try
            {
                // Gọi service để kiểm tra tất cả các cảm biến của người dùng
                await _sensorDataService.CheckSensorsForUserAsync(request.UserId, request.Date);
                return Ok(new { Message = "Sensor check completed successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Error: {ex.Message}" });
            }
        }
    }
    
}
