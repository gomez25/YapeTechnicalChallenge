namespace AntiFraudService.Domain.DTOs
{
  public class TransactionStatusDto
  {
    public Guid TransactionId { get; set; }
    public string Status { get; set; } = string.Empty;
  }
}
