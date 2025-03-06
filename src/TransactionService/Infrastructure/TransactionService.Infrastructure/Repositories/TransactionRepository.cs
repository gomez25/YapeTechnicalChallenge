using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using TransactionService.Domain.DTOs;
using TransactionService.Domain.Entities;
using TransactionService.Domain.Interfaces;
using TransactionService.Infrastructure.Database;

namespace TransactionService.Infrastructure.Repositories
{
    internal class TransactionRepository(TransactionContext dbContext) : ITransactionRepository
    {
        private readonly TransactionContext _dbContext = dbContext;

        public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
        {
            await _dbContext.Transactions.AddAsync(transaction);
            await _dbContext.SaveChangesAsync();
            return transaction;
        }

        public async Task<TransactionDto> GetTransactionWithTotalAmountAsync(Guid transactionSourceId)
        {
            var today = DateTime.UtcNow.Date;

            var transactionDto = await (
                from t in _dbContext.Transactions
                where t.SourceAccountId == transactionSourceId
                select new TransactionDto
                {
                    Id = t.Id,
                    Amount = t.Amount,
                    TotalAmount = (
                        from x in _dbContext.Transactions
                        where x.SourceAccountId == t.SourceAccountId && x.CreatedAt.Date == today
                        select (decimal?)x.Amount
                    ).Sum() ?? 0,
                    Status = t.Status
                }
            ).FirstOrDefaultAsync();

            if (transactionDto is null)
                throw new Exception("Transaction not found");

            return transactionDto;
        }


        public async Task<bool> UpdateStatusAsync(TransactionStatusDto transaction)
        {
            var result = await _dbContext.Transactions
                .Where(t => t.Id == transaction.TransactionId)
                .FirstOrDefaultAsync() ?? throw new Exception("Transaction not found");

            result.Status = transaction.Status;

            _dbContext.Transactions.Update(result);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
