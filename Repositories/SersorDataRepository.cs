using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenIotApi.Models;
using Google.Cloud.Firestore;
using GreenIotApi.Repositories.IRepositories;


namespace GreenIotApi.Repositories
{
    public class SensorDataRepository : Repository<SensorData>, ISensorDataRepository
    {
        private readonly FirestoreDb _firestoreDb;
        

        public SensorDataRepository(FirestoreDb firestoreDb) : base(firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }

        public async Task<string> AddSensorDataAsync(string gardenId, SensorData sensorData)
        {
            var dataCollection = _firestoreDb.Collection("gardens").Document(gardenId).Collection("data");
            var documentRef = await dataCollection.AddAsync(sensorData);
            return documentRef.Id;
        }

        

        public async Task<SensorData> GetLatestSensorDataAsync(string gardenId)
        {
            if (string.IsNullOrEmpty(gardenId))
            {
                Console.WriteLine("GardenId is null or empty.");
                return null;
            }

            var dataCollection = _firestoreDb
                .Collection("gardens")
                .Document(gardenId)
                .Collection("data");

          

            var query = dataCollection
                .OrderByDescending("Timestamp") // Đảm bảo "timestamp" là đúng
                .Limit(1);

            var snapshot = await query.GetSnapshotAsync();

            Console.WriteLine($"Snapshot count: {snapshot.Count}");

            if (!snapshot.Documents.Any())
            {
                Console.WriteLine("No documents found in subcollection 'data'.");
                return null;
            }

            var latestData = snapshot.Documents.First().ConvertTo<SensorData>();
            Console.WriteLine($"Latest data retrieved: {latestData.Timestamp}, Temperature: {latestData.Temperature}");

            return latestData;
        }

        public async Task<List<SensorData>> GetSensorDataByTimeRangeAsync(string gardenId, DateTime start, DateTime end)
        {
            var collection = _firestoreDb.Collection("gardens").Document(gardenId).Collection("data");
            var query = collection.WhereGreaterThanOrEqualTo("Timestamp", start).WhereLessThanOrEqualTo("Timestamp", end);
            var snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<SensorData>()).ToList();
        }
        public async Task<List<SensorData>> GetSensorDataAsync(string nodeId, int year, int month, int? day)
        {
            try
            {
                var collectionRef = _firestoreDb.Collection("gardens").Document(nodeId).Collection("data");

                // Tạo DateTime cho đầu tháng và cuối tháng, chuyển sang UTC
                var startOfMonth = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc); // Chuyển sang UTC
                var endOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month), 23, 59, 59, 999, DateTimeKind.Utc); // Chuyển sang UTC

                var query = collectionRef.WhereGreaterThanOrEqualTo("Timestamp", startOfMonth)
                                        .WhereLessThan("Timestamp", endOfMonth);

                // Nếu có ngày cụ thể, thêm điều kiện lọc theo ngày
                if (day.HasValue)
                {
                    var startOfDay = new DateTime(year, month, day.Value, 0, 0, 0, DateTimeKind.Utc); // Chuyển sang UTC
                    var endOfDay = new DateTime(year, month, day.Value, 23, 59, 59, 999, DateTimeKind.Utc); // Kết thúc ngày là 23:59:59.999 (UTC)
                    query = query.WhereGreaterThanOrEqualTo("Timestamp", startOfDay)
                                .WhereLessThanOrEqualTo("Timestamp", endOfDay);
                }

               
                query = query.OrderBy("Timestamp");

                var snapshot = await query.GetSnapshotAsync();

                // Chuyển đổi dữ liệu từ Firestore thành kiểu SensorData
                return snapshot.Documents.Select(doc => doc.ConvertTo<SensorData>()).ToList();
            }
            catch (System.Exception ex)
            {
                throw new System.Exception($"Error while fetching data: {ex.Message}");
            }
        }




        private DateTime GetStartOfWeek(int year, int month, int weekNumber)
        {
            // Ngày đầu tháng
            var firstDayOfMonth = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);  // Sử dụng UTC cho ngày đầu tháng

            // Điều chỉnh sao cho ngày đầu tuần là Thứ Hai (0 = Monday, 1 = Tuesday, ..., 6 = Sunday)
            int daysToSubtract = (int)firstDayOfMonth.DayOfWeek - 1; // Đảm bảo rằng tuần bắt đầu từ Thứ Hai
            if (daysToSubtract < 0)
            {
                daysToSubtract = 6; // Nếu ngày đầu tháng là Chủ nhật, thì lấy ngày thứ Hai tuần trước
            }

            // Lấy ngày đầu tuần (bắt đầu từ Thứ Hai) trong UTC
            var startOfWeek = firstDayOfMonth.AddDays(-daysToSubtract);

            // Tính toán ngày đầu tuần của tuần mong muốn (tuầnNumber là tuần bắt đầu từ 1)
            var startOfWeekForWeekNumber = startOfWeek.AddDays((weekNumber - 1) * 7);

            // Trả về ngày bắt đầu của tuần trong UTC
            return startOfWeekForWeekNumber;
        }


        public async Task<List<SensorData>> GetSensorDataByMonthAsync(string nodeId, int year, int month)
        {
            try
            {
                var collectionRef = _firestoreDb.Collection("gardens").Document(nodeId).Collection("data");

                // Tạo DateTime cho đầu và cuối tháng, chuyển sang UTC
                var startOfMonth = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc); // Từ đầu tháng
                var endOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month), 23, 59, 59, 999, DateTimeKind.Utc); // Đến cuối tháng

                // Truy vấn Firestore
                var query = collectionRef.WhereGreaterThanOrEqualTo("Timestamp", startOfMonth)
                                        .WhereLessThan("Timestamp", endOfMonth);

                query = query.OrderBy("Timestamp");

                var snapshot = await query.GetSnapshotAsync();

                // Chuyển dữ liệu thành kiểu SensorData
                return snapshot.Documents.Select(doc => doc.ConvertTo<SensorData>()).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while fetching data for month {month}, year {year}: {ex.Message}");
            }
        }

        
        public async Task<List<SensorData>> GetSensorDataByWeekAsync(string nodeId, int year, int month, int week)
        {
            try
            {
                var collectionRef = _firestoreDb.Collection("gardens").Document(nodeId).Collection("data");

                // Xác định ngày bắt đầu và kết thúc của tuần (lấy theo UTC)
                var startOfWeek = GetStartOfWeek(year, month, week);
                var endOfWeek = startOfWeek.AddDays(7); // Ngày kết thúc là 7 ngày sau đó

                // Lọc dữ liệu trong phạm vi tuần đã xác định
                var query = collectionRef
                    .WhereGreaterThanOrEqualTo("Timestamp", startOfWeek)
                    .WhereLessThan("Timestamp", endOfWeek)
                    .OrderBy("Timestamp");

                var snapshot = await query.GetSnapshotAsync();

                // Trả về dữ liệu đã lấy từ Firestore
                return snapshot.Documents.Select(doc => doc.ConvertTo<SensorData>()).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while fetching data for week {week}: {ex.Message}");
            }
        }


    // private DateTime GetStartOfWeek(int year, int month, int weekNumber)
    // {
    //     var firstDayOfMonth = new DateTime(year, month, 1);
    //     int daysToAdd = (weekNumber - 1) * 7 - (int)firstDayOfMonth.DayOfWeek;
    //     return firstDayOfMonth.AddDays(daysToAdd);
    // }

    }
}