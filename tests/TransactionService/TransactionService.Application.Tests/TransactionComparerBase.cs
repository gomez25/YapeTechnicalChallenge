using TransactionService.Domain.Entities;

public class TransactionComparerBase
{
    public bool Equals(Transaction x, Transaction y)
    {
        if (x == null || y == null)
            return false;

        return x.Amount == y.Amount &&
               x.SourceAccountId == y.SourceAccountId &&
               x.TargetAccountId == y.TargetAccountId &&
               x.Status == y.Status;
    }
}