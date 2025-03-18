namespace Darts_Score_Management.Interfaces.RepositoryInterfaces
{
    public interface IBaseRepository<T>
    {

        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}

