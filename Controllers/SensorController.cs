using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenIotApi.Models;
using GreenIotApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using GreenIotApi.Services;
using GreenIotApi.DTOs;
using AutoMapper;
namespace GreenIotApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorController : ControllerBase
    {
        private readonly SensorDataRepository _sensorDataRepository;
        private readonly ILogger<SensorController> _logger;
        private readonly FirestoreService _firestoreService;
        private readonly IMapper _mapper;

        public SensorController(FirestoreService firestoreService,SensorDataRepository sensorDataRepository, ILogger<SensorController> logger, IMapper mapper)
        {
            _firestoreService = firestoreService;
            _sensorDataRepository = sensorDataRepository;
            _logger = logger;
            _mapper = mapper;
        }   

        [HttpPost("add")]
        public async Task<IActionResult> AddSensorData([FromBody] SensorDataDto sensorData)
        {
            try
            {
                var mappedSensorData = _mapper.Map<SensorData>(sensorData);
                await _firestoreService.AddSensorDataAsync(mappedSensorData);
                return Ok(new { message = "Data added successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

    }
}