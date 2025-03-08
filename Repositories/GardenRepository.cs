using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using GreenIotApi.Models;
using GreenIotApi.Repositories.IRepositories;
using GreenIotApi.Repositories;

namespace GreenIotApi.Repositories
{
    public class GardenRepository: Repository<Garden>, IGardenRepository
    {
        private readonly FirestoreDb _firestoreDb;
        public GardenRepository(FirestoreDb firestoreDb) : base(firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }
        public async Task<List<Garden>> GetGardensByUserIdAsync(string userId)
        {
            var dataCollection = _firestoreDb
                .Collection("gardens")
                .WhereEqualTo("user_id", userId);

            var snapshot = await dataCollection.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Garden>()).ToList();
        }
        public async Task<SensorData> GetDataSensorGardenAsync(string gardenId)
        {
            if (string.IsNullOrEmpty(gardenId))
                throw new ArgumentException("GardenId cannot be null or empty.");

            var dataCollection = _firestoreDb
                .Collection("gardens")
                .Document(gardenId)
                .Collection("data");

            // Truy vấn lấy tài liệu gần nhất
            var query = dataCollection
                .OrderByDescending("timestamp") // Sắp xếp theo timestamp giảm dần
                .Limit(1);                      // Chỉ lấy tài liệu đầu tiên

            var snapshot = await query.GetSnapshotAsync();

            // Nếu không có tài liệu nào
            if (!snapshot.Documents.Any())
            {
                return null;
            }

            // Chuyển đổi tài liệu đầu tiên thành SensorData
            return snapshot.Documents.First().ConvertTo<SensorData>();
        }

        public async Task<List<Garden>> FilterGardensByUserIdAndGardenIdAsync(string userId, string name)
        {
            var dataCollection = _firestoreDb
                .Collection("gardens")
                .WhereEqualTo("user_id", userId)
                .WhereEqualTo("name", name);

            var snapshot = await dataCollection.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Garden>()).ToList();
        }
    }
}