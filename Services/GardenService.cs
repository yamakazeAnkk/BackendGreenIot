using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GreenIotApi.Models;
using GreenIotApi.Repositories;
using GreenIotApi.DTOs;
using GreenIotApi.Repositories.IRepositories;
using GreenIotApi.Services.InterfaceService;

namespace GreenIotApi.Services
{
    public class GardenService
    {
        private readonly IGardenRepository _gardenRepository;

        private readonly IFirebaseStorageService _firebaseStorageService;
        private readonly IMapper _mapper;

        public GardenService(IGardenRepository gardenRepository, IFirebaseStorageService firebaseStorageService, IMapper mapper)
        {
            _gardenRepository = gardenRepository;
            _firebaseStorageService = firebaseStorageService;
            _mapper = mapper;
        }

        public async Task<bool> CheckGardenExistsAsync(string gardenId)
        {
            var garden = await _gardenRepository.GetAsync(gardenId);
            return garden != null;
        }

        public async Task<List<GardenDto>> GetGardensByUserIdAsync(string userId)
        {
            var gardens = await _gardenRepository.GetGardensByUserIdAsync(userId);
            return _mapper.Map<List<GardenDto>>(gardens);
        }

        public async Task<GardenDto> AddGardenAsync(Garden garden, Stream imageStream, string fileName, string contentType)
        {
            // Upload ảnh nếu có
            if (imageStream != null && !string.IsNullOrEmpty(fileName))
            {
                string imageUrl = await _firebaseStorageService.UploadFileAsync(imageStream, fileName, contentType);
                garden.GardenImage = imageUrl; // Gán URL public vào đối tượng Garden
            }

            // Lưu thông tin garden vào Firestore
            var gardenId = await _gardenRepository.AddAsync(garden);
            var addedGarden = await _gardenRepository.GetAsync(gardenId);

            return _mapper.Map<GardenDto>(addedGarden);
        }

        public async Task<bool> UpdateGardenAsync(string id, Garden garden)
        {
            return await _gardenRepository.UpdateAsync(id, garden);
        }

        public async Task<bool> DeleteGardenAsync(string id)
        {
            return await _gardenRepository.DeleteAsync(id);
        }

        public async Task<GardenDto> GetGardenAsync(string id)
        {
            var garden = await _gardenRepository.GetAsync(id);
            return _mapper.Map<GardenDto>(garden);
        }

        public async Task<List<Garden>> GetAllGardensAsync()
        {
            var gardens = await _gardenRepository.GetAllAsync();
            return gardens;
        }

        public async Task<SensorDataDto> GetDataSensorGardenAsync(string gardenId)
        {
            var latestData = await _gardenRepository.GetDataSensorGardenAsync(gardenId);

            if (latestData == null)
            {
                return null;
            }

            // Mapping từ SensorData sang SensorDataDto
            return _mapper.Map<SensorDataDto>(latestData);
        }

        public async Task<List<Garden>> FilterGardensByUserIdAndGardenIdAsync(string userId, string name)
        {
            return await _gardenRepository.FilterGardensByUserIdAndGardenIdAsync(userId, name);
        }
        
    }
}