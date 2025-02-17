using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GreenIotApi.DTOs;
using GreenIotApi.Models;
using GreenIotApi.Repositories;
using GreenIotApi.Services;
using GreenIotApi.Models;
using GreenIotApi.Repositories.IRepositories;

namespace GreenIotApi.Services
{
    public class DeviceService
    {
        private readonly DeviceRepository _deviceRepository;
        private readonly GardenService _gardenService;
        private readonly IMapper _mapper;

        public DeviceService(DeviceRepository deviceRepository, GardenService gardenService, IMapper mapper)
        {
            _deviceRepository = deviceRepository;
            _gardenService = gardenService;
            _mapper = mapper;
        }

        public async Task<string> AddDeviceAsync(string gardenId, Device device)
        {
            if (!await _gardenService.CheckGardenExistsAsync(gardenId))
                throw new ArgumentException("Garden does not exist.");
            
            return await _deviceRepository.AddDeviceAsync(gardenId, device);
        }

        public async Task<List<DeviceDto>> GetDevicesAsync(string gardenId)
        {
            var devices = await _deviceRepository.GetDevicesAsync(gardenId);
            return _mapper.Map<List<DeviceDto>>(devices);
        }
        public async Task<bool> CheckGardenExistsAsync(string gardenId)
        {
            return await _gardenService.CheckGardenExistsAsync(gardenId);
        }
    }
}