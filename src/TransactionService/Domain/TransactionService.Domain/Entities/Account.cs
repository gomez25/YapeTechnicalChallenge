namespace TransactionService.Domain.Entities
{
    public class Account
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<Transaction> SourceTransactions { get; set; } = [];
        public ICollection<Transaction> TargetTransactions { get; set; } = [];
    }
}
