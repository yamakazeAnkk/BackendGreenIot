using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using GreenIotApi.Repositories.IRepositories;

namespace GreenIotApi.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly FirestoreDb _firestoreDb;
        private readonly string _collectionName;

        public Repository(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
            _collectionName = typeof(T).Name.ToLower() + "s";
        }

        public async Task<string> AddAsync(T entity)
        {
            var collection = _firestoreDb.Collection(_collectionName);
            var documentRef = await collection.AddAsync(entity);
            return documentRef.Id;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var document = _firestoreDb.Collection(_collectionName).Document(id);
            var snapshot = await document.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                await document.DeleteAsync();
                return true;

            }
            return false;
        }

        public async Task<List<T>> GetAllAsync()
        {
            var collection = await _firestoreDb.Collection(_collectionName).GetSnapshotAsync();
            return collection.Documents.Select(doc => doc.ConvertTo<T>()).ToList();
        }

        public async Task<T> GetAsync(string id)
        {
            var document = await _firestoreDb.Collection(_collectionName).Document(id).GetSnapshotAsync();
            return document.Exists ? document.ConvertTo<T>() : null;
        }

        public async Task<bool> UpdateAsync(string id, T entity)
        {
            var document = _firestoreDb.Collection(_collectionName).Document(id);
            var snapshot = await document.GetSnapshotAsync();

            if (!snapshot.Exists)
                return false;

            await document.SetAsync(entity, SetOptions.Overwrite);
            return true;
        }
    }
}