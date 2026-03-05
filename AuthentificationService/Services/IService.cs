namespace AuthentificationService.Services
{
    public interface IService<Tsend, Treceive>
    {
        Task<Tsend> Create(Treceive receive);
        Task<Tsend> GetById(int id);
        Task<List<Tsend>> GetAll();
    }
}
