using TransactionService.Domain.DTOs;
using TransactionService.Domain.Entities;
using TransactionService.Domain.Shared;

namespace TransactionService.Domain.Interfaces
{
  public interface ITransactionService
  {
    Task<Response<Transaction>> AddTransactionAsync(AddTransactionDto transaction);
  }
}
