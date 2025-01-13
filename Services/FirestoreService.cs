using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace GreenIotApi.Services
{
    public class FirestoreService
    {
        private readonly FirestoreDb _firestoreDb;

        public FirestoreService()
        {
            string path = "bookstore-59884-firebase-adminsdk-p59pi-005bba2668.json"; 
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            _firestoreDb = FirestoreDb.Create("bookstore-59884"); 
        }

        public async Task AddSensorDataAsync(Models.SensorData sensorData)
        {
            var collection = _firestoreDb.Collection("SensorData");
            await collection.AddAsync(sensorData);
        }

    }
}