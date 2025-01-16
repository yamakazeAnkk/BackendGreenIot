using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Google.Cloud.Firestore;
using GreenIotApi.Models;
using GreenIotApi.DTOs;

namespace GreenIotApi.Services
{
    public class FirestoreService
    {
        private readonly FirestoreDb _firestoreDb;
        private readonly IMapper _mapper;

        public FirestoreService(IMapper mapper)
        {
            string path = "bookstore-59884-firebase-adminsdk-p59pi-005bba2668.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            _firestoreDb = FirestoreDb.Create("bookstore-59884");
            _mapper = mapper;
        }

        // Add Device to a Garden
        public async Task<string> AddDeviceAsync(string gardenId, Device device)
        {
            if (string.IsNullOrEmpty(gardenId))
                throw new ArgumentException("GardenId cannot be null or empty.");
            
            if(!await CheckGardenExistsAsync(gardenId))
                throw new ArgumentException("GardenId does not exist.");
            var deviceCollection = _firestoreDb.Collection("gardens").Document(gardenId).Collection("devices");
            var documentRef = await deviceCollection.AddAsync(device); // Auto-generate DeviceId
            return documentRef.Id;
        }

        // Get All Devices from a Garden
        public async Task<List<DeviceDto>> GetDevicesAsync(string gardenId)
        {
            if (string.IsNullOrEmpty(gardenId))
                throw new ArgumentException("GardenId cannot be null or empty.");

            
            if(GetGardenByUserIdAsync(gardenId) == null)
                throw new ArgumentException("GardenId does not exist.");
            var deviceCollection = _firestoreDb.Collection("gardens").Document(gardenId).Collection("devices");
            var snapshot = await deviceCollection.GetSnapshotAsync();

            var devices = snapshot.Documents.Select(doc => doc.ConvertTo<Device>()).ToList();
            return _mapper.Map<List<DeviceDto>>(devices);
        }

        // Add SensorData to a Garden
        public async Task<string> AddSensorDataAsync(string gardenId, SensorData sensorData)
        {
            if (string.IsNullOrEmpty(gardenId))
                throw new ArgumentException("GardenId cannot be null or empty.");

            var dataCollection = _firestoreDb.Collection("gardens").Document(gardenId).Collection("data");
            var documentRef = await dataCollection.AddAsync(sensorData); // Auto-generate SensorDataId
            return documentRef.Id;
        }

        // Get All SensorData from a Garden
        public async Task<List<SensorDataDto>> GetSensorDataAsync(string gardenId)
        {
            if (string.IsNullOrEmpty(gardenId))
                throw new ArgumentException("GardenId cannot be null or empty.");

            var dataCollection = _firestoreDb.Collection("gardens").Document(gardenId).Collection("data");
            var snapshot = await dataCollection.GetSnapshotAsync();

            var sensorDataList = snapshot.Documents.Select(doc => doc.ConvertTo<SensorData>()).ToList();
            return _mapper.Map<List<SensorDataDto>>(sensorDataList);
        }
        public async Task<Garden> GetGardenAsync(string gardenId)
        {
            if (string.IsNullOrEmpty(gardenId))
                throw new ArgumentException("GardenId cannot be null or empty.");
            
            var garden = await _firestoreDb.Collection("gardens").Document(gardenId).GetSnapshotAsync();
            return garden.ConvertTo<Garden>();
        }

        public async Task<bool> CheckGardenExistsAsync(string gardenId)
        {
            var garden = await GetGardenAsync(gardenId);
            return garden != null;
        }
        public async Task<List<Garden>> GetGardenByUserIdAsync(string userId)
        {
            var garden = await _firestoreDb.Collection("gardens").WhereEqualTo("userId", userId).GetSnapshotAsync();
            return garden.Documents.Select(doc => doc.ConvertTo<Garden>()).ToList();
        }
    }
}