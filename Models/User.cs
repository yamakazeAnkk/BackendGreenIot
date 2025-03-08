using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
namespace GreenIotApi.Models
{
    [FirestoreData]
    public class User
    {
        [FirestoreProperty]
        public string? Id { get; set; }
        [FirestoreProperty]
        public string? Name { get; set; }
        [FirestoreProperty]
        public string? DateOfBirth { get; set; }
        [FirestoreProperty]
        public string? Phone { get; set; }
        [FirestoreProperty]
        public string? Image { get; set; }
        [FirestoreProperty]
        public string? Role { get; set; }
    }
}