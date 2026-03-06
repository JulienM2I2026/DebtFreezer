using Gateway.Enums;

namespace Gateway.Dtos
{
    public class DebtReceive
    {
        public int UserId { get; set; }
        public string Creditor { get; set; }
        public double OriginalAmount { get; set; }
        public double InterestRate { get; set; }
        public DateTime DueDate { get; set; }
        public DebtType Type { get; set; }
        public DebtStatus Status { get; set; }
    }
}
