using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenIotApi.Repositories.IRepositories
{
    public interface IRepository <T>
    {
        Task<T> GetAsync(string id);
        Task<List<T>> GetAllAsync();
        Task<string> AddAsync(T entity);
        Task<bool> UpdateAsync(string id, T entity);
        Task<bool> DeleteAsync(string id);
    }
}