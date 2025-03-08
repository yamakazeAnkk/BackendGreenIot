using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
namespace GreenIotApi.DTOs
{
    [FirestoreData]
    public class EmailDto
    {
        
        [FirestoreProperty]
        public string To { get; set; }   
        [FirestoreProperty]
        public string Subject { get; set; } 
        [FirestoreProperty]
        public string Body { get; set; }
    }

}