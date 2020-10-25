using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.DAL.Interfaces
{
    public interface IAsyncRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task CreateAsync(T item);

        Task UpdateAsync(T item);

        Task DeleteAsync(int id);
    }
}
