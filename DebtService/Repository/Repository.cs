namespace DebtService.Repository
{
    public class Repository : IRepository<Order>
    {

        private AppDbContext _db;

        public Repository(AppDbContext appDb)
        {
            _db = appDb;
        }
        public Order Create(Order entity)
        {
            _db.Add(entity);
            _db.SaveChanges();
            return entity;
        }

        public List<Order> GetAll()
        {
            return _db.Orders.ToList();
        }

        public Order GetById(int id)
        {
            return _db.Orders.Find(id);
        }
    }
}
