using DebtService.Dtos;
using DebtService.Enums;
using DebtService.Models;
using DebtService.Repository;
using DebtService.RestClient;

namespace DebtService.Services
{
    public class Service : IService<DebtSend, DebtReceive>
    {

        private readonly IRepository<Debt> _repository;
        private readonly Client<DebtSend> _client;

        public Service(IRepository<Debt> repository)
        {
            _repository = repository;
            _client = new Client<DebtSend>("http://localhost:5066/api/Product/");
        }

        public async Task<DebtSend> Create(DebtReceive receive)
        {
            Debt debt = DtoToEntity(receive);
            await _repository.CreateAsync(debt);
            return await EntityToDto(debt);
        }

        public async Task<List<DebtSend>> GetAll()
        {
            List<Debt> debts = await _repository.GetAllAsync();
            List<DebtSend> debtSends = new List<DebtSend>();
            foreach (Debt send in debts)
            {
                debtSends.Add(await EntityToDto(send));
            }
            return debtSends;
        }

        public async Task<DebtSend> GetById(int id)
        {
            Debt? debt = await _repository.GetByIdAsync(id);

            if (debt == null)
                throw new Exception("Debt not found");

            return await EntityToDto(debt);
        }


        private Debt DtoToEntity(DebtReceive receive)
        {
            return new Debt() { UserId = receive.UserId, Creditor = receive.Creditor, OriginalAmount = receive.OriginalAmount, InterestRate = receive.InterestRate, DueDate = receive.DueDate, Type = receive.Type, Status = receive.Status };
        }


        private async Task<DebtSend> EntityToDto(Debt debt)
        {
            DebtSend send = new DebtSend() { Id = debt.Id, UserId = debt.UserId, Creditor = debt.Creditor, OriginalAmount = debt.OriginalAmount, InterestRate = debt.InterestRate, DueDate = debt.DueDate, Type = debt.Type, Status = debt.Status };

            // Il faudra mettre le UserName plus tard

            return send;
        }
    }
}
