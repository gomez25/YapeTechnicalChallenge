using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using TransactionService.Domain.DTOs;
using TransactionService.Domain.Interfaces;

namespace TransactionService.Infrastructure.Kafka
{
    public class TransactionStatusConsumer : BackgroundService
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly ITransactionRepository _transactionRepository;

        public TransactionStatusConsumer(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
            var transactionTopic = Environment.GetEnvironmentVariable("KAFKA_STATUS_TOPIC") ?? "transaction-status";
            var kafkaBroker = Environment.GetEnvironmentVariable("KAFKA_BROKER") ?? "kafka:9092";

            var config = new ConsumerConfig
            {
                BootstrapServers = kafkaBroker,
                GroupId = Environment.GetEnvironmentVariable("KAFKA_STATUS_CONSUMER_GROUP") ?? "status-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                SocketTimeoutMs = 10000,
                SessionTimeoutMs = 30000,
                MetadataMaxAgeMs = 300000,
                ReconnectBackoffMs = 500,
                ReconnectBackoffMaxMs = 10000
            };

            _consumer = new ConsumerBuilder<string, string>(config).Build();

            bool subscribed = false;
            int retries = 5;
            while (!subscribed && retries > 0)
            {
                try
                {
                    _consumer.Subscribe(transactionTopic);
                    subscribed = true;
                    Console.WriteLine($"Successfully subscribed to topic '{transactionTopic}'");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error subscribing to Kafka topic '{transactionTopic}': {ex.Message}");
                    retries--;
                    Task.Delay(5000).Wait();
                }
            }
            if (!subscribed)
            {
                throw new Exception($"Failed to subscribe to Kafka topic '{transactionTopic}' after multiple attempts.");
            }
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(stoppingToken);

                    if (consumeResult != null && !string.IsNullOrEmpty(consumeResult.Message.Value))
                    {
                        var statusUpdate = JsonSerializer.Deserialize<TransactionStatusDto>(consumeResult.Message.Value);

                        if (statusUpdate != null)
                        {
                            await _transactionRepository.UpdateStatusAsync(statusUpdate);
                        }
                    }
                }
                catch (ConsumeException ex)
                {
                    Console.WriteLine($"Kafka Consumer Error: {ex.Error.Reason}");
                    await Task.Delay(5000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Kafka Consumer Canceled.");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected Error in Kafka Consumer: {ex.Message}");
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _consumer.Close();
            await base.StopAsync(cancellationToken);
        }
    }
}
