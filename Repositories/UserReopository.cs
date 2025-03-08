using Google.Cloud.Firestore;
using GreenIotApi.Models;
using System;
using System.Threading.Tasks;

namespace GreenIotApi.Repositories
{
    public class UserRepository
    {
        private readonly FirestoreDb _firestoreDb;

        public UserRepository(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }

        // Thêm người dùng mới vào Firestore với documentId do client truyền vào
        public async Task<User> AddUserAsync(User user)
        {
            try
            {
                // Sử dụng id của client làm documentId
                DocumentReference docRef = _firestoreDb.Collection("users").Document(user.Id); // Client truyền id vào

                // Thêm dữ liệu người dùng vào Firestore với documentId là user.Id
                await docRef.SetAsync(user);

                // Trả về đối tượng người dùng đã được thêm và chứa documentId
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding user to Firestore", ex);
            }
        }
        public async Task<User> GetUserByIdAsync(string documentId)
        {
            var userCollection = _firestoreDb.Collection("users");
            var documentRef = userCollection.Document(documentId);
            var snapshot = await documentRef.GetSnapshotAsync();
            return snapshot.ConvertTo<User>();
        }
    }
}
