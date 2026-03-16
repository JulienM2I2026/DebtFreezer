namespace DebtService.Dtos
{
    public class DebtSummaryDto
    {
        public double TotalOriginalAmount { get; set; }
        public double TotalRemainingAmount { get; set; }
        public double TotalPaidAmount { get; set; }
        public int ActiveDebtsCount { get; set; }
        public int PaidDebtsCount { get; set; }
        public double AverageInterestRate { get; set; }
        public double TotalAccruedInterest { get; set; }
    }
}
