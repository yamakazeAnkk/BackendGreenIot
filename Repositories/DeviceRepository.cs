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