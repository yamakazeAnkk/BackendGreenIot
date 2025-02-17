using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using AutoMapper;
using GreenIotApi.Models;
using GreenIotApi.Repositories;
using GreenIotApi.DTOs;
using GreenIotApi.Repositories.IRepositories;
using GreenIotApi.DTOs.ModelViews;

namespace GreenIotApi.Services
{
    public class SensorDataService
    {
        private readonly ISensorDataRepository _sensorDataRepository;
        private readonly GardenService _gardenService;
        private readonly IMapper _mapper;

        public SensorDataService(ISensorDataRepository sensorDataRepository, GardenService gardenService, IMapper mapper)
        {
            _sensorDataRepository = sensorDataRepository;
            _gardenService = gardenService;
            _mapper = mapper;
        }

        public async Task<string> AddSensorDataAsync(string gardenId, SensorData sensorData)
        {
            if (!await _gardenService.CheckGardenExistsAsync(gardenId))
                throw new ArgumentException("Garden does not exist.");
            
            return await _sensorDataRepository.AddSensorDataAsync(gardenId, sensorData);
        }

        
        public async Task<SensorDataDto> GetLatestSensorDataAsync(string gardenId)
        {
            var latestData = await _sensorDataRepository.GetLatestSensorDataAsync(gardenId);
            return _mapper.Map<SensorDataDto>(latestData);
        }
        public async Task<List<DataChartModelView>> GetDailySensorDataAsync(string gardenId,DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1).AddSeconds(-1);
            var data = await _sensorDataRepository.GetSensorDataByTimeRangeAsync(gardenId, startDate, endDate);
            return _mapper.Map<List<DataChartModelView>>(data);
        }
        public async Task<List<DataChartModelView>> GetWeeklySensorDataAsync(string gardenId,DateTime weekStart)
        {
            var startDate = weekStart.Date;
            var endDate = startDate.AddDays(7).AddSeconds(-1);
            var data = await _sensorDataRepository.GetSensorDataByTimeRangeAsync(gardenId, startDate, endDate);
            return _mapper.Map<List<DataChartModelView>>(data);
        }
        public async Task<List<DataChartModelView>> GetMonthlySensorDataAsync(string gardenId,DateTime monthStart)
        {
            var startDate = monthStart.Date;
            var endDate = startDate.AddMonths(1).AddSeconds(-1);
            var data = await _sensorDataRepository.GetSensorDataByTimeRangeAsync(gardenId, startDate, endDate);
            return _mapper.Map<List<DataChartModelView>>(data);
        }
        public async Task<List<DataChartModelView>> GetYearlySensorDataAsync(string gardenId,int year, int month)
        {
            var start = new DateTime(year, month, 1); 
            var end = start.AddMonths(1).AddTicks(-1); 
            var data = await _sensorDataRepository.GetSensorDataByTimeRangeAsync(gardenId, start, end);
            return _mapper.Map<List<DataChartModelView>>(data);
        }
        public async Task<Dictionary<string, List<float>>> GetTimeSeriesDataAsync(string nodeId, int year, int month, int? day,string columnName)
        {
            var sensorData = await _sensorDataRepository.GetSensorDataAsync(nodeId, year, month, day);

            
            if (!IsValidColumnName(columnName))
            {
                throw new ArgumentException("Invalid column name provided.");
            }
            var groupedData = GroupDataByTwoHourInterval(sensorData, year, month, day);

            // Prepare the time-series data for all fields, filling missing data with 0
            var timeSeriesData = new Dictionary<string, List<float>>
            {
                { "SoilMoisture", FillMissingData(groupedData.Select(group => group.Average(d => d.SoilMoisture)).ToList()) },
                { "Temperature", FillMissingData(groupedData.Select(group => group.Average(d => d.Temperature)).ToList()) },
                { "Humidity", FillMissingData(groupedData.Select(group => group.Average(d => d.Humidity)).ToList()) },
                { "LightLevel", FillMissingData(groupedData.Select(group => group.Average(d => d.LightLevel)).ToList()) },
                { "CoPpm", FillMissingData(groupedData.Select(group => group.Average(d => d.CoPpm)).ToList()) }
            };

            return new Dictionary<string, List<float>> { { columnName, timeSeriesData[columnName] } };
        }
        private bool IsValidColumnName(string columnName)
        {
            var validColumnNames = new List<string> { "SoilMoisture", "Temperature", "Humidity", "LightLevel", "CoPpm" };
            return validColumnNames.Contains(columnName);
        }

        private List<IGrouping<DateTime, SensorData>> GroupDataByTwoHourInterval(List<SensorData> data, int year, int month, int? day)
        {
            var filteredData = data.Where(d => d.Timestamp.HasValue &&
                                            d.Timestamp.Value.Year == year &&
                                            d.Timestamp.Value.Month == month &&
                                            (day == null || d.Timestamp.Value.Day == day)).ToList();

            // Nhóm dữ liệu theo 2 giờ
            var groupedData = filteredData
                .GroupBy(d => new DateTime(d.Timestamp.Value.Year, d.Timestamp.Value.Month, d.Timestamp.Value.Day, 
                                        (d.Timestamp.Value.Hour / 3) * 3, 0, 0)) // Nhóm theo mỗi 2 giờ
                .ToList();

            // Sắp xếp nhóm theo thời gian (từ 00:00 đến 24:00)
            return groupedData.OrderBy(g => g.Key).ToList();
        }

  
        private List<float> FillMissingData(List<float> data)
        {
         
            var expectedCount = 8; 

          
            var filledData = new List<float>(new float[expectedCount]);

          
            for (int i = 0; i < data.Count; i++)
            {
                filledData[i] = data[i];
            }

            return filledData;
        }
        public async Task<List<float>> GetMonthlyAverageByWeekAsync(string nodeId, int year, int month, string columnName)
        {
            // Get all sensor data for the given month
            var sensorData = await _sensorDataRepository.GetSensorDataByMonthAsync(nodeId, year, month);

            // Prepare the weekly averages (there will be 4 or 5 weeks in the month)
            var weeklyAverages = new List<float> { 0, 0, 0, 0, 0 }; // Default values for 5 weeks

            // Group data by week (divide into 5 weeks if the month has 30 or 31 days)
            var weeksData = GroupDataByWeek(sensorData, year, month);

            // Calculate weekly averages
            for (int i = 0; i < weeksData.Count; i++)
            {
                // Ensure that we do not access out of range
                if (i < weeklyAverages.Count)
                {
                    // If the group has data, calculate the average
                    if (weeksData[i].Any())
                    {
                        float weekAverage = 0;
                        switch (columnName.ToLower())
                        {
                            case "soilmoisture":
                                weekAverage = weeksData[i].Average(d => d.SoilMoisture);
                                break;
                            case "temperature":
                                weekAverage = weeksData[i].Average(d => d.Temperature);
                                break;
                            case "humidity":
                                weekAverage = weeksData[i].Average(d => d.Humidity);
                                break;
                            case "lightlevel":
                                weekAverage = weeksData[i].Average(d => d.LightLevel);
                                break;
                            case "coppm":
                                weekAverage = weeksData[i].Average(d => d.CoPpm);
                                break;
                            default:
                                throw new ArgumentException($"Column '{columnName}' is not valid.");
                        }

                        weeklyAverages[i] = weekAverage;
                    }
                    else
                    {
                        // If the week has no data, set it to 0 to indicate no data for that week
                        weeklyAverages[i] = 0;
                    }
                }
            }

            return weeklyAverages; // Return the weekly averages as a flat list
        }


    // Method to group data by 4 weeks of the month
        private List<List<SensorData>> GroupDataByWeek(List<SensorData> data, int year, int month)
        {
            var weeks = new List<List<SensorData>> { new List<SensorData>(), new List<SensorData>(), new List<SensorData>(), new List<SensorData>(), new List<SensorData>() };

            // Get the first and last day of the month
            var firstDayOfMonth = new DateTime(year, month, 1);
            var lastDayOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));

            // Calculate total days in the month
            var daysInMonth = (lastDayOfMonth - firstDayOfMonth).Days + 1;  // Including the last day

            // Divide the month into 5 weeks (if necessary)
            int daysPerWeek = daysInMonth / 5; // Calculate days per week, with 5 weeks if necessary

            foreach (var item in data)
            {
                if (item.Timestamp.HasValue)
                {
                    var dayOfMonth = item.Timestamp.Value.Day;
                    int weekIndex = (dayOfMonth - 1) / daysPerWeek; // Determine which week the day belongs to

                    if (weekIndex < weeks.Count)
                    {
                        weeks[weekIndex].Add(item); // Add the item to the correct week
                    }
                }
            }

            return weeks;
        }
        public async Task<List<float>> GetWeeklyDataByColumnAsync(string nodeId, int year, int month, int week, string columnName)
        {
            // Lấy tất cả dữ liệu cho tuần đã cho
            var sensorData = await _sensorDataRepository.GetSensorDataByWeekAsync(nodeId, year, month, week);

            // Mảng để lưu trữ giá trị trung bình của từng ngày trong tuần
            var weeklyAverages = new List<float> { 0, 0, 0, 0, 0, 0, 0 }; // Khởi tạo 7 giá trị cho 7 ngày, mặc định là 0

            // Nhóm dữ liệu theo ngày trong tuần
            var groupedDataByDay = GroupDataByDay(sensorData, year, month, week);

            // Tính trung bình cho mỗi ngày trong tuần
            for (int i = 0; i < groupedDataByDay.Count; i++)
            {
                if (groupedDataByDay[i].Any())
                {
                    float dayAverage = 0;
                    switch (columnName.ToLower())
                    {
                        case "soilmoisture":
                            dayAverage = groupedDataByDay[i].Average(d => d.SoilMoisture);
                            break;
                        case "temperature":
                            dayAverage = groupedDataByDay[i].Average(d => d.Temperature);
                            break;
                        case "humidity":
                            dayAverage = groupedDataByDay[i].Average(d => d.Humidity);
                            break;
                        case "lightlevel":
                            dayAverage = groupedDataByDay[i].Average(d => d.LightLevel);
                            break;
                        case "coppm":
                            dayAverage = groupedDataByDay[i].Average(d => d.CoPpm);
                            break;
                        default:
                            throw new ArgumentException($"Column '{columnName}' is not valid.");
                    }

                    weeklyAverages[i] = dayAverage;
                }
                else
                {
                    // Nếu không có dữ liệu cho ngày đó, thay thế bằng 0
                    weeklyAverages[i] = 0;
                }
            }

            return weeklyAverages; // Trả về mảng trung bình cho 7 ngày
        }

    // Nhóm dữ liệu theo ngày trong tuần
        private List<List<SensorData>> GroupDataByDay(List<SensorData> data, int year, int month, int week)
        {
            var daysInWeek = new List<List<SensorData>> { new List<SensorData>(), new List<SensorData>(), new List<SensorData>(), new List<SensorData>(), new List<SensorData>(), new List<SensorData>(), new List<SensorData>() };

            // Lấy ngày bắt đầu của tuần
            var startOfWeek = GetStartOfWeek(year, month, week);

            foreach (var item in data)
            {
                if (item.Timestamp.HasValue)
                {
                    // Lấy ngày trong tuần (0 = Thứ Hai, 1 = Thứ Ba, ..., 6 = Chủ Nhật)
                    var dayOfWeek = item.Timestamp.Value.DayOfWeek;
                    daysInWeek[(int)dayOfWeek].Add(item); // Nhóm dữ liệu theo từng ngày trong tuần
                }
            }

            return daysInWeek;
        }
        private DateTime GetStartOfWeek(int year, int month, int weekNumber)
        {
            var firstDayOfMonth = new DateTime(year, month, 1);
            int daysToAdd = (weekNumber - 1) * 7 - (int)firstDayOfMonth.DayOfWeek;
            return firstDayOfMonth.AddDays(daysToAdd);
        }


        

    }
    
}