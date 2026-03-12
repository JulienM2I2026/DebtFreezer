namespace PaymentService.Dtos
{
    public class DebtReceive
    {
        public Guid UserId { get; set; }
        public string Creditor { get; set; }
        public double OriginalAmount { get; set; }
        public double RemainingAmount { get; set; }
        public double InterestRate { get; set; }
        public DateTime DueDate { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
    }
}
