using Confluent.Kafka;
using Moq;
using System.Text.Json;
using TransactionService.Domain.DTOs;
using TransactionService.Infrastructure.Kafka;

namespace TransactionService.Infrastructure.Tests
{
    public class TransactionServiceProducerTests
    {
        [Fact]
        public async Task PublishTransactionAsync_ShouldProduceMessage()
        {
            // Arrange
            var mockProducer = new Mock<IProducer<string, string>>();

            var transaction = new TransactionDto
            {
                Id = Guid.NewGuid(),
                Amount = 100,
                TotalAmount = 200,
                Status = "PENDING"
            };

            var expectedMessage = JsonSerializer.Serialize(transaction);

            mockProducer
                .Setup(p => p.ProduceAsync(
                    It.IsAny<string>(),
                    It.IsAny<Message<string, string>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DeliveryResult<string, string>
                {
                    TopicPartitionOffset = new TopicPartitionOffset("transaction-topic", new Partition(0), new Offset(1)),
                    Message = new Message<string, string>
                    {
                        Key = transaction.Id.ToString(),
                        Value = expectedMessage
                    }
                });

            Environment.SetEnvironmentVariable("KAFKA_BROKER", "localhost:9092");
            Environment.SetEnvironmentVariable("KAFKA_TRANSACTION_TOPIC", "transaction-topic");

            var transactionServiceProducer = new TransactionServiceProducer();
            var producerField = typeof(TransactionServiceProducer)
                .GetField("_producer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (producerField != null)
            {
                producerField.SetValue(transactionServiceProducer, mockProducer.Object);
            }

            // Act
            await transactionServiceProducer.PublishTransactionAsync(transaction);

            mockProducer.Verify(p => p.ProduceAsync(
                "transaction-topic",
                It.Is<Message<string, string>>(m => m.Key == transaction.Id.ToString() && m.Value == expectedMessage),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
