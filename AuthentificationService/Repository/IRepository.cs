namespace AuthentificationService.Repository
{
    public interface IRepository<T>
    {
        Task<T?> GetByIdAsync(object id);
        Task<List<T>> GetAllAsync();

        IQueryable<T> Query(); // pour filtres/tri/pagination au niveau service

        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);

        Task SaveChangesAsync();
    }
}