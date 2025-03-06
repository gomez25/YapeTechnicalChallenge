namespace TransactionService.Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public decimal Amount { get; set; }
        public Guid SourceAccountId { get; set; }
        public Guid TargetAccountId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int TransferTypeId { get; set; }

        public TransferType TransferType { get; set; } = null!;
        public Account SourceAccount { get; set; } = null!;
        public Account TargetAccount { get; set; } = null!;
    }
}
