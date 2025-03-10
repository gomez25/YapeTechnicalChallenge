using System.Collections.Generic;
using TransactionService.Domain.Entities;

public class TransactionComparer : IEqualityComparer<Transaction>
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

    public int GetHashCode(Transaction obj)
    {
        return HashCode.Combine(obj.Amount, obj.SourceAccountId, obj.TargetAccountId, obj.Status);
    }
}

