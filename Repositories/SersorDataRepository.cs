using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenIotApi.Models;
using MongoDB.Driver;

namespace GreenIotApi.Repositories
{
    public class SensorDataRepository
    {
        private readonly IMongoCollection<SensorData> _sensorDataCollection;
        
        public SensorDataRepository(IMongoDatabase database)
        {
            _sensorDataCollection = database.GetCollection<SensorData>("SensorData");
        }

        public async Task AddSensorDataAsync(SensorData data)
        {
            await _sensorDataCollection.InsertOneAsync(data);
        }
    }
}