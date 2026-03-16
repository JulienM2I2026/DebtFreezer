namespace DebtService.Dtos
{
    public class DebtByMonthDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int DebtCount { get; set; }
        public double TotalOriginalAmount { get; set; }
    }
}
