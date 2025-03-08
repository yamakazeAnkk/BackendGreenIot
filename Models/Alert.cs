using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
namespace GreenIotApi.Models
{
    [FirestoreData]
    public class Alert
    {
        [FirestoreDocumentId]
        public string AlertId { get; set; } 

        [FirestoreProperty]
        public string UserId { get; set; }  

        [FirestoreProperty]
        public string GardenId { get; set; } 

        [FirestoreProperty]
        public string Message { get; set; } 

        [FirestoreProperty]
        public DateTime Timestamp { get; set; } 

        [FirestoreProperty]
        public bool Resolved { get; set; }
    }
}