using TransactionService.Domain.DTOs;
using TransactionService.Domain.Entities;

namespace TransactionService.Domain.Interfaces
{
  public interface ITransactionRepository
  {
    Task<Transaction> CreateTransactionAsync(Transaction transaction);
    Task<TransactionDto> GetTransactionWithTotalAmountAsync(Guid transactionSourceId);
    Task<bool> UpdateStatusAsync(TransactionStatusDto transaction);
  }
}
