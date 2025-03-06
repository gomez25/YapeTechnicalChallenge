namespace TransactionService.Domain.DTOs
{
  public class TransactionDto
  {
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
  }
}
