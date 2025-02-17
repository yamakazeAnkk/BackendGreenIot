using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenIotApi.Models;

namespace GreenIotApi.Repositories.IRepositories
{
    public interface ISensorDataRepository:IRepository<SensorData>
    {
        Task<string> AddSensorDataAsync(string gardenId, SensorData sensorData);
        Task<SensorData> GetLatestSensorDataAsync(string gardenId);
        Task<List<SensorData>> GetSensorDataAsync(string nodeId, int year, int month, int? day);
        Task<List<SensorData>> GetSensorDataByMonthAsync(string nodeId, int year, int month);
        Task<List<SensorData>> GetSensorDataByWeekAsync(string nodeId, int year, int month, int week);
        Task<List<SensorData>> GetSensorDataByTimeRangeAsync(string gardenId, DateTime start, DateTime end);
        

    

    }
}