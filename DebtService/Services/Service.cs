using DebtService.Dtos;
using DebtService.Models;
using DebtService.Repository;
using System.Text.Json;

namespace DebtService.Services
{
    public class Service : IService<DebtSend, DebtReceive>
    {

        private readonly IRepository<Debt> _repository;

        public Service(IRepository<Debt> repository)
        {
            _repository = repository;
        }



        public async Task<DebtSend> Create(DebtReceive receive)
        {
            receive.RemainingAmount = receive.OriginalAmount;
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


        public async Task<List<DebtSend>> GetAllByUserId(Guid userId)
        {
            var debts = await _repository.GetByUserIdAsync(userId);
            Console.WriteLine(JsonSerializer.Serialize(debts)); // voir toutes les propriétés
            return debts.Select(d => new DebtSend
            {
                Id = d.Id,
                UserId = d.UserId,
                Creditor = d.Creditor,
                InterestRate = d.InterestRate,
                Type = d.Type,
                OriginalAmount = d.OriginalAmount,
                RemainingAmount = d.RemainingAmount,
                Status = d.Status,
                DueDate = d.DueDate,
            }).ToList();
        }


        public async Task<DebtSend> GetById(int id)
        {
            Debt? debt = await _repository.GetByIdAsync(id);

            if (debt == null)
                throw new Exception("Debt not found");

            return await EntityToDto(debt);
        }

        public async Task<DebtSend> GetByUserAndDebtId(Guid userId, int debtId)
        {
            Debt? debt = await _repository.GetByUserAndDebtIdAsync(userId, debtId);
            if (debt == null)
                throw new Exception("Debt not found");

            return await EntityToDto(debt);
        }

        public async Task<bool> UpdateDebtAmount(int id, double amount)
        {
            Debt? debt = await _repository.GetByIdAsync(id);
            if (debt == null || debt.Status == 1 || debt.RemainingAmount < amount)
                return false;
            debt.RemainingAmount -= amount;
            if(debt.RemainingAmount == 0)
                debt.Status = 1;

            await _repository.UpdateAsync(debt);
            return true;
        }


        private Debt DtoToEntity(DebtReceive receive)
        {
            return new Debt() { UserId = receive.UserId, Creditor = receive.Creditor, OriginalAmount = receive.OriginalAmount, InterestRate = receive.InterestRate, DueDate = receive.DueDate, Type = receive.Type, Status = receive.Status, RemainingAmount = receive.RemainingAmount };
        }


        private async Task<DebtSend> EntityToDto(Debt debt)
        {
            DebtSend send = new DebtSend() { Id = debt.Id, UserId = debt.UserId, Creditor = debt.Creditor, OriginalAmount = debt.OriginalAmount, InterestRate = debt.InterestRate, DueDate = debt.DueDate, Type = debt.Type, Status = debt.Status, RemainingAmount = debt.RemainingAmount };

            // Il faudra mettre le UserName plus tard

            return send;
        }
    }
}
