using System.Linq.Expressions;

namespace EcommerceDotNetCore.Repository;

public interface IRepository<T> where T : class
{
    public Task<T> FindByIdAsync(int id);
    public Task<IEnumerable<T>> GetAllAsync();

    public Task AddAsync(T entity);

    public Task UpdateAsync(T entity);

    public Task DeleteAsync(T entity);

}