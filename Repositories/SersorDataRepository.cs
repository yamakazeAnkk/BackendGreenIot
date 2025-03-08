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




        private DateTime GetStartOfWeek(DateTime date)
        {
            // Tính toán ngày thứ 2 của tuần mà ngày đó thuộc về
            var dayOfWeek = (int)date.DayOfWeek;
            var daysToSubtract = (dayOfWeek == 0) ? 6 : dayOfWeek - 1; // Nếu là Chủ Nhật (0), trừ 6 ngày, nếu là ngày khác, trừ để về thứ 2
            var startOfWeek = date.AddDays(-daysToSubtract); // Ngày bắt đầu của tuần (thứ 2)
            return startOfWeek;
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

        
        public async Task<List<SensorData>> GetSensorDataByWeekAsync(string nodeId, int year, int month, int day)
        {
            try
                {
                    // Tạo ngày từ năm, tháng và ngày đã nhập
                    DateTime inputDate = new DateTime(year, month, day);

                    // Lấy ngày bắt đầu (thứ 2) và ngày kết thúc (chủ nhật) của tuần
                    DateTime startOfWeek = GetStartOfWeek(inputDate).ToUniversalTime();
                    DateTime endOfWeek = startOfWeek.AddDays(7); // Ngày kết thúc là 7 ngày sau ngày bắt đầu

                    var collectionRef = _firestoreDb.Collection("gardens").Document(nodeId).Collection("data");

                    // Truy vấn Firestore để lấy dữ liệu trong phạm vi từ thứ 2 đến chủ nhật
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
                throw new Exception($"Error while fetching data for date {day}: {ex.Message}");
            }

        }

        public Task CheckAndAlertForSensorActivityAsync(Garden garden, DateTime date)
        {
            throw new NotImplementedException();
        }


        // private DateTime GetStartOfWeek(int year, int month, int weekNumber)
        // {
        //     var firstDayOfMonth = new DateTime(year, month, 1);
        //     int daysToAdd = (weekNumber - 1) * 7 - (int)firstDayOfMonth.DayOfWeek;
        //     return firstDayOfMonth.AddDays(daysToAdd);
        // }
        public async Task<List<SensorData>> GetSensorDataByDateAsync(string gardenId, DateTime date)
        {
            try
            {
                // Chuyển đổi DateTime thành UTC
                var startOfDay = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc); // 00:00:00 UTC
                var endOfDay = startOfDay.AddDays(1); // 23:59:59 UTC

                // Truy vấn Firestore để lấy dữ liệu cảm biến trong khoảng thời gian ngày cần tìm
                var collectionRef = _firestoreDb
                    .Collection("gardens")
                    .Document(gardenId)  // Truy vấn theo gardenId
                    .Collection("data");  // Dữ liệu cảm biến nằm trong sub-collection "data"

                // Truy vấn các document có Timestamp nằm trong khoảng thời gian của ngày
                var query = collectionRef
                    .WhereGreaterThanOrEqualTo("Timestamp", startOfDay)
                    .WhereLessThan("Timestamp", endOfDay)
                    .OrderBy("Timestamp");

                var snapshot = await query.GetSnapshotAsync();

                // Chuyển các document lấy được thành đối tượng SensorData
                var sensorDataList = snapshot.Documents
                    .Select(doc => doc.ConvertTo<SensorData>())
                    .ToList();

                return sensorDataList;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                throw new Exception($"Error fetching sensor data for garden {gardenId} on {date.ToShortDateString()}: {ex.Message}");
            }
        }

        public Task CheckSensorsForUserAsync(DateTime date)
        {
            throw new NotImplementedException();
        }
    }
}