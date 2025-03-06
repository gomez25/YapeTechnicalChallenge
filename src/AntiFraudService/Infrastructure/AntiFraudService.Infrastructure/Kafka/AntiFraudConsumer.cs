using AntiFraudService.Domain.DTOs;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace AntiFraudService.Infrastructure.Kafka
{
    public class AntiFraudConsumer : BackgroundService
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly IProducer<string, string> _producer;
        private readonly string _statusTopic;

        public AntiFraudConsumer()
        {
            _statusTopic = Environment.GetEnvironmentVariable("KAFKA_STATUS_TOPIC") ?? "transaction-status";
            var transactionTopic = Environment.GetEnvironmentVariable("KAFKA_TRANSACTION_TOPIC") ?? "transaction-topic";
            var kafkaBroker = Environment.GetEnvironmentVariable("KAFKA_BROKER") ?? "kafka:9092";

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = kafkaBroker,
                GroupId = "anti-fraud-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                SocketTimeoutMs = 10000,
                SessionTimeoutMs = 30000,
                MetadataMaxAgeMs = 300000,
                ReconnectBackoffMs = 500,
                ReconnectBackoffMaxMs = 10000
            };

            _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = kafkaBroker,
                Acks = Acks.All,
                MessageTimeoutMs = 10000,
                MessageSendMaxRetries = int.MaxValue,
                RetryBackoffMs = 500
            };

            _producer = new ProducerBuilder<string, string>(producerConfig).Build();

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
                    var transaction = JsonSerializer.Deserialize<TransactionDto>(consumeResult.Message.Value);

                    if (transaction == null) continue;

                    bool isFraudulent = CheckFraud(transaction);
                    transaction.Status = isFraudulent ? "REJECTED" : "APPROVED";

                    var statusMessage = JsonSerializer.Serialize(new TransactionStatusDto
                    {
                        TransactionId = transaction.Id,
                        Status = transaction.Status
                    });

                    await _producer.ProduceAsync(_statusTopic, new Message<string, string>
                    {
                        Key = transaction.Id.ToString(),
                        Value = statusMessage
                    });

                    _consumer.Commit(consumeResult);
                }
                catch (ConsumeException ex)
                {
                    Console.WriteLine($"Kafka Consume Error: {ex.Error.Reason}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private static bool CheckFraud(TransactionDto transaction)
        {
            return transaction.Amount > 2000 || transaction.TotalAmount > 20000;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _consumer.Close();
            _producer.Flush(cancellationToken);
            await base.StopAsync(cancellationToken);
        }
    }
}
