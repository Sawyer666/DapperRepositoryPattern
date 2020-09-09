using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DapperRepositoryPattern
{
    public interface IGenericRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task DeleteRowAsync(int id);
        Task<T> GetAsync(int id);      
        Task UpdateAsync(T t);
        Task InsertAsync(T t);
    }
}
