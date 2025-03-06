using TransactionService.Domain.DTOs;
using TransactionService.Domain.Entities;
using TransactionService.Domain.Interfaces;
using TransactionService.Domain.Shared;

namespace TransactionService.Application
{
    public class TransactionService(IKafkaProducer kafkaProducer, ITransactionRepository transactionRepository) : ITransactionService
    {
        private readonly IKafkaProducer _kafkaProducer = kafkaProducer;
        private readonly ITransactionRepository _transactionRepository = transactionRepository;

        public async Task<Response<Transaction>> AddTransactionAsync(AddTransactionDto transaction)
        {
            var newTransaction = new Transaction
            {
                SourceAccountId = transaction.SourceAccountId,
                TargetAccountId = transaction.TargetAccountId,
                Amount = transaction.Amount,
                Status = "PENDING"
            };

            var finalTransaction = await _transactionRepository.GetTransactionWithTotalAmountAsync(transaction.SourceAccountId);

            await _transactionRepository.CreateTransactionAsync(newTransaction);

            await _kafkaProducer.PublishTransactionAsync(finalTransaction);

            return new Response<Transaction>()
            {
                IsSuccess = true,
                Data = newTransaction,
                Message = "Transaction processed",
                StatusCode = 200
            };
        }
    }
}
