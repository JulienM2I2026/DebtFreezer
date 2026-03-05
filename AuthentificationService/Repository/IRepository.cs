namespace AuthentificationService.Repository
{
    public interface IRepository<T>
    {

        T GetById(int id);
        List<T> GetAll();
        T Create(T entity);
    }
}
