namespace TransactionService.Domain.DTOs
{
    public class AddTransactionDto
    {
        public decimal Amount { get; set; }
        public Guid SourceAccountId { get; set; }
        public Guid TargetAccountId { get; set; }
    }
}
