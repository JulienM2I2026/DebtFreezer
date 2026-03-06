using DebtService.Enums;
using System.ComponentModel.DataAnnotations;

namespace DebtService.Dtos
{
    public class DebtSend
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Creditor { get; set; }
        public double OriginalAmount { get; set; }
        public double InterestRate { get; set; }
        public DateTime DueDate { get; set; }
        public DebtType Type { get; set; }
        public DebtStatus Status { get; set; }
    }
}


