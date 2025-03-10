using Confluent.Kafka;
using System.Text.Json;
using TransactionService.Domain.DTOs;
using TransactionService.Domain.Interfaces;

namespace TransactionService.Infrastructure.Kafka
{
    public class TransactionServiceProducer : IKafkaProducer
    {
        private readonly IProducer<string, string> _producer;
        private readonly string _topic;

        public TransactionServiceProducer()
        {
            var config = new ProducerConfig
            {
                BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BROKER") ?? "kafka:9092",
                Acks = Acks.All,
                MessageTimeoutMs = 5000,
                SecurityProtocol = SecurityProtocol.Plaintext
            };

            _producer = new ProducerBuilder<string, string>(config).Build();
            _topic = Environment.GetEnvironmentVariable("KAFKA_TRANSACTION_TOPIC") ?? "transaction-topic";
        }

        public async Task PublishTransactionAsync(TransactionDto transaction)
        {
            var message = JsonSerializer.Serialize(transaction);
            await _producer.ProduceAsync(_topic, new Message<string, string>
            {
                Key = transaction.Id.ToString(),
                Value = message
            });

            _producer.Flush();
        }
    }
}
