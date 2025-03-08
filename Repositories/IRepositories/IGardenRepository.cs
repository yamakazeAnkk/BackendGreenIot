using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenIotApi.Models;
namespace GreenIotApi.Repositories.IRepositories
{
    public interface IGardenRepository: IRepository<Garden>
    {
        Task<SensorData> GetDataSensorGardenAsync(string gardenId);
        Task<List<Garden>> GetGardensByUserIdAsync(string userId);

        Task<List<Garden>> FilterGardensByUserIdAndGardenIdAsync(string userId,string name);

    }
}