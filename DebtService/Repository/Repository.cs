using DebtService.Data;
using DebtService.Models;

namespace DebtService.Repository
{
    public class Repository : IRepository<Debt>
    {

        private AppDbContext _db;

        public Repository(AppDbContext appDb)
        {
            _db = appDb;
        }
        public Debt Create(Debt entity)
        {
            _db.Add(entity);
            _db.SaveChanges();
            return entity;
        }

        public List<Debt> GetAll()
        {
            return _db.Debts.ToList();
        }

        public Debt GetById(int id)
        {
            return _db.Debts.Find(id);
        }
    }
}
