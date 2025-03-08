using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace GreenIotApi.Services
{
    public class DailyCheckService : BackgroundService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DailyCheckService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Đảm bảo chỉ chạy vào lúc 00:00 mỗi ngày
                var currentTime = DateTime.UtcNow;
                var nextRunTime = DateTime.UtcNow.Date.AddDays(1); // Thời gian chạy tiếp theo là lúc 00:00 ngày hôm sau

                var delay = nextRunTime - currentTime; // Tính toán thời gian chờ

                if (delay.TotalMilliseconds > 0)
                {
                    await Task.Delay(delay, stoppingToken); // Chờ đến 00:00 của ngày mới
                }

                // Gửi POST request vào lúc 00:00
                await SendPostRequest();

                // Chờ một ngày (24 giờ) để chạy lại
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }

        private async Task SendPostRequest()
        {
            var request = new
            {
                UserId = "someUserId",  // ID người dùng có thể lấy từ nơi khác
                Date = DateTime.UtcNow.ToString("yyyy-MM-dd")  // Gửi ngày hiện tại
            };

            try
            {
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.PostAsync(
                "http://localhost:8080/api/check-sensors", // URL của API
                new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
                );

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Sensor check completed successfully.");
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in sending POST request: {ex.Message}");
            }
        }
    }

}