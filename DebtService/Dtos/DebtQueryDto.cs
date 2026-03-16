namespace DebtService.Dtos
{
    public class DebtQueryDto
    {
        public Guid UserId { get; set; }

        // Filtres optionnels
        public int? Status { get; set; }       // 0 = Active, 1 = Paid
        public int? Type { get; set; }          // type de dette
        public string? Creditor { get; set; }   // recherche partielle sur le créancier

        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
