using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
namespace GreenIotApi.Models
{
    [FirestoreData]
    public class Garden
    {
        [FirestoreDocumentId]
        public string GardenId { get; set; }

        [FirestoreProperty("name")]
        public string Name { get; set; }

        [FirestoreProperty("garden_image")]
        public string GardenImage { get; set; }

        [FirestoreProperty("user_id")]
        public string UserId { get; set; }

        [FirestoreProperty("location")]
        public string Location { get; set; }
    }
}