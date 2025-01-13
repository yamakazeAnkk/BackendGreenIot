using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace GreenIotApi.Models
{
    [FirestoreData]
    public class SensorData
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }
        [FirestoreProperty]
        public float Temperature { get; set; }
        [FirestoreProperty]
        public float Humidity { get; set; }
        [FirestoreProperty]
        public float SoilMoisture { get; set; }
        [FirestoreProperty]
        public float LightLevel { get; set; }
        [FirestoreProperty]
        public float CoPpm { get; set; }
        [FirestoreProperty]
        public float IsRaining { get; set; }
        [FirestoreProperty]
        public DateTime? Timestamp { get; set; }  = DateTime.UtcNow;
    }
}