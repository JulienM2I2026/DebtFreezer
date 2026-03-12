namespace PaymentService.Dtos;

public class PaymentDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string? Notes { get; set; }
    public DebtSend? Debt { get; set; }
}
