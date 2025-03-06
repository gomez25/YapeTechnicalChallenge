namespace AntiFraudService.Domain.Interfaces
{
  public interface IKafkaConsumerService
  {
    Task ConsumeTransactionsAsync(CancellationToken stoppingToken);
  }
}
