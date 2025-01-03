using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GreenIotApi.Models
{
    public class SensorData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public float Temperature { get; set; }
        public float Humidity { get; set; }
        public float SoilMoisture { get; set; }
        public float LightLevel { get; set; }
        public float CoPpm { get; set; }
        public float IsRaining { get; set; }
        public DateTime Timestamp { get; set; }  = DateTime.Now;
    }
}