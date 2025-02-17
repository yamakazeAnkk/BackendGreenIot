using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using GreenIotApi.Services.InterfaceService;
using Google.Cloud.Storage.V1;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using GreenIotApi.Models;
using System.Text;
namespace GreenIotApi.Services
{
    public class FirebaseStorageService : IFirebaseStorageService
    {
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;
        private readonly string _firebaseDatabaseUrl = "https://bookstore-59884-default-rtdb.firebaseio.com/"; // Your Firebase Realtime Database URL

        public FirebaseStorageService()
        {
            string credentialPath = "bookstore-59884-firebase-adminsdk-p59pi-005bba2668.json";

            // Kiểm tra nếu file thông tin xác thực không tồn tại
            if (!File.Exists(credentialPath))
            {
                throw new InvalidOperationException($"Credential file does not exist at: {credentialPath}");
            }

            GoogleCredential credential = GoogleCredential.FromFile(credentialPath);
            _storageClient = StorageClient.Create(credential);

            // Tên bucket Firebase Storage của bạn
            _bucketName = "bookstore-59884.appspot.com";
        }

        // Method to upload file to Firebase Storage
        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            try
            {
                var objectName = $"images/{fileName}";

                // Upload file to Firebase Storage
                var imageObject = await _storageClient.UploadObjectAsync(
                    bucket: _bucketName,
                    objectName: objectName,
                    contentType: contentType,
                    source: fileStream
                );

                // Fetch metadata for download token
                var objMetadata = _storageClient.GetObject(_bucketName, objectName);
                string downloadToken = "";

                if (objMetadata.Metadata != null && objMetadata.Metadata.ContainsKey("firebaseStorageDownloadTokens"))
                {
                    downloadToken = objMetadata.Metadata["firebaseStorageDownloadTokens"];
                }
                else
                {
                    // Create a new token if not exists
                    downloadToken = Guid.NewGuid().ToString();
                    var updateMetadata = _storageClient.PatchObject(new Google.Apis.Storage.v1.Data.Object
                    {
                        Bucket = _bucketName,
                        Name = objectName,
                        Metadata = new System.Collections.Generic.Dictionary<string, string>
                        {
                            { "firebaseStorageDownloadTokens", downloadToken }
                        }
                    });
                }

                // Generate public URL to access the image
                var firebaseUrl = $"https://firebasestorage.googleapis.com/v0/b/{_bucketName}/o/{Uri.EscapeDataString(objectName)}?alt=media&token={downloadToken}";

                return firebaseUrl;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to upload image to Firebase Storage: {ex.Message}");
            }
        }

        // Method to fetch data from Firebase Realtime Database
        public async Task<FirebaseData> GetDataFromRealtimeDatabaseAsync(string nodeId)
        {
            using (var httpClient = new HttpClient())
            {
                // Construct the Firebase Realtime Database URL
                var url = $"{_firebaseDatabaseUrl}/{nodeId}.json";

                try
                {
                    var response = await httpClient.GetStringAsync(url);
                    var data = JsonConvert.DeserializeObject<FirebaseData>(response); // Deserialize into FirebaseData object

                    return data; // Return the fetched data
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to fetch data from Firebase Realtime Database: {ex.Message}");
                }
            }
        }
        public async Task UpdateDataToRealtimeDatabaseAsync(string nodeId, FirebaseData data){
            using (var httpClient = new HttpClient())
            {
                var url = $"{_firebaseDatabaseUrl}/{nodeId}.json";
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

                try
                {
                    // Send a PATCH request to update the data
                    var response = await httpClient.PatchAsync(url, content);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Failed to update data in Firebase: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to update data in Firebase: {ex.Message}");
                }
            }
        }
        
    }
}
