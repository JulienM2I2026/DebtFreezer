namespace DebtService.Repository
{
    public interface IRepository<T>
    {

        Task<T?> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<T> CreateAsync(T entity);
    }
}
