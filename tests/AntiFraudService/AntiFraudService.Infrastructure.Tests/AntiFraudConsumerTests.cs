using AntiFraudService.Domain.DTOs;
using AntiFraudService.Infrastructure.Kafka;
using Confluent.Kafka;
using Moq;
using System.Reflection;
using System.Text.Json;
using Xunit;

namespace AntiFraudService.Infrastructure.Tests
{
    public class AntiFraudConsumerTests
    {
        private readonly Mock<IConsumer<string, string>> _mockConsumer;
        private readonly Mock<IProducer<string, string>> _mockProducer;
        private readonly AntiFraudConsumer _antiFraudConsumer;

        public AntiFraudConsumerTests()
        {
            _mockConsumer = new Mock<IConsumer<string, string>>();
            _mockProducer = new Mock<IProducer<string, string>>();

            _antiFraudConsumer = new AntiFraudConsumer();

            // Manually inject the mocks into the private fields
            typeof(AntiFraudConsumer)
                .GetField("_consumer", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(_antiFraudConsumer, _mockConsumer.Object);

            typeof(AntiFraudConsumer)
                .GetField("_producer", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(_antiFraudConsumer, _mockProducer.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ProcessesValidTransaction()
        {
            var transaction = new TransactionDto { Id = Guid.NewGuid(), Amount = 1000, TotalAmount = 5000 };
            var transactionMessage = JsonSerializer.Serialize(transaction);
            var consumeResult = new ConsumeResult<string, string>
            {
                Message = new Message<string, string> { Key = transaction.Id.ToString(), Value = transactionMessage },
                TopicPartitionOffset = new TopicPartitionOffset("transaction-topic", 0, 0)
            };

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

            _mockConsumer.SetupSequence(c => c.Consume(It.IsAny<CancellationToken>()))
                .Returns(consumeResult)
                .Throws(new OperationCanceledException());

            _mockProducer.Setup(p => p.ProduceAsync("transaction-status", It.IsAny<Message<string, string>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DeliveryResult<string, string> { Status = PersistenceStatus.Persisted });

            var task = _antiFraudConsumer.StartAsync(cts.Token);
            await Task.Delay(500);
            cts.Cancel();
            await task;

            _mockProducer.Verify(p => p.ProduceAsync("transaction-status", It.IsAny<Message<string, string>>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockConsumer.Verify(c => c.Commit(It.IsAny<ConsumeResult<string, string>>()), Times.Once);
        }

        [Theory]
        [InlineData(500, 5000, "APPROVED")]
        [InlineData(3000, 25000, "REJECTED")]
        public void CheckFraud_ReturnsCorrectStatus(decimal amount, decimal totalAmount, string expectedStatus)
        {
            // Arrange
            var transaction = new TransactionDto { Amount = amount, TotalAmount = totalAmount };

            // Act
            bool isFraudulent = AntiFraudConsumer.CheckFraud(transaction);

            // Assert
            Assert.Equal(expectedStatus, isFraudulent ? "REJECTED" : "APPROVED");
        }
    }
}
