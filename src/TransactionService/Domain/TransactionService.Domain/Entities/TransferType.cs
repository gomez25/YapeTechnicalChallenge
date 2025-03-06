namespace TransactionService.Domain.Entities
{
    public class TransferType
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;

        public ICollection<Transaction> Transactions { get; set; } = [];
    }

}
