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
    public class AlertRepository
    {
        private readonly FirestoreDb _firestoreDb;
        public AlertRepository(FirestoreDb firestoreDb) 
        {
            _firestoreDb = firestoreDb;
        }
        public async Task CreateAlertAsync(string userId, string gardenId, string message, DateTime timestamp)
        {
            var alert = new Alert
            {
                UserId = userId,
                GardenId = gardenId,
                Message = message,
                Timestamp = timestamp,
                Resolved = false // Cảnh báo chưa được giải quyết
            };

            // Thêm cảnh báo vào Firestore
            await _firestoreDb.Collection("alerts").AddAsync(alert);
        }
        public async Task<List<Alert>> GetAlertsByUserIdAsync(string userId)
        {
            try
            {
                var alerts = new List<Alert>();

                // Query the alerts collection where the userId matches
                var query = _firestoreDb.Collection("alerts")
                    .WhereEqualTo("UserId", userId);

                var snapshot = await query.GetSnapshotAsync();

                foreach (var doc in snapshot.Documents)
                {
                    var alert = doc.ConvertTo<Alert>();
                    alerts.Add(alert);
                }

                // Sort alerts by Timestamp
                alerts = alerts.OrderBy(alert => alert.Timestamp).ToList();

                return alerts;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching alerts for user {userId}: {ex.Message}");
            }
        }
        
    }
}