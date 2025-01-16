using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace GreenIotApi.Repositories
{
    public class FirestoreContext
    {
        public readonly FirestoreDb _firestoreDb;
      
        public FirestoreContext()
        {
            // Cấu hình đường dẫn đến file JSON credential
            string path = "bookstore-59884-firebase-adminsdk-p59pi-005bba2668.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            // Tạo đối tượng FirestoreDb
            _firestoreDb = FirestoreDb.Create("bookstore-59884");
          
        }
    }
}