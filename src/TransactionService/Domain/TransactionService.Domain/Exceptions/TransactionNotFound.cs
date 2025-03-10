namespace TransactionService.Domain.Exceptions
{
    public class TransactionNotFound : Exception
    {
        public int StatusCode { get; }

        public TransactionNotFound() : base("Transaction not found")
        {
            StatusCode = 404;
        }
    }
}
