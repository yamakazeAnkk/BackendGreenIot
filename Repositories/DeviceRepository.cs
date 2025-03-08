using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using GreenIotApi.Models;

namespace GreenIotApi.Repositories.IRepositories
{
    public class DeviceRepository
    {
        
        private readonly FirestoreDb _firestoreDb;

        public DeviceRepository(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }

        public async Task<string> AddDeviceAsync(string gardenId, Device device)
        {
            var deviceCollection = _firestoreDb.Collection("gardens").Document(gardenId).Collection("devices");

            // Kiểm tra xem thiết bị đã có trong cơ sở dữ liệu chưa
            var existingDevice = await deviceCollection
                .WhereEqualTo("Name", device.Name)
                .GetSnapshotAsync();

            if (existingDevice.Documents.Any())
            {
                throw new InvalidOperationException($"Device with the name '{device.Name}' already exists.");
            }

            // Thêm thiết bị vào Firestore nếu chưa có
            var documentRef = await deviceCollection.AddAsync(device);
            return documentRef.Id;
        }



        public async Task<List<Device>> GetDevicesAsync(string gardenId)
        {
            var deviceCollection = _firestoreDb.Collection("gardens").Document(gardenId).Collection("devices");
            var snapshot = await deviceCollection.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Device>()).ToList();
        }
        

    }
}