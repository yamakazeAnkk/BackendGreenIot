using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
namespace GreenIotApi.Models
{
    [FirestoreData]
    public class Device
    {
        
        [FirestoreDocumentId]
        public string DeviceId { get; set; }

        [FirestoreProperty("name")]
        public string Name { get; set; }

    }
}