using TransactionService.Domain.DTOs;

namespace TransactionService.Domain.Interfaces
{
  public interface IKafkaProducer
  {
    Task PublishTransactionAsync(TransactionDto transaction);
  }
}
