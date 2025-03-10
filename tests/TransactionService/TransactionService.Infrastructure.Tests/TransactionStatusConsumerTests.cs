using Confluent.Kafka;
using Moq;
using System.Text.Json;
using TransactionService.Domain.DTOs;
using TransactionService.Domain.Interfaces;
using TransactionService.Infrastructure.Kafka;
using Xunit;

namespace TransactionService.Infrastructure.Tests
{
    public class TransactionStatusConsumerTests
    {
        private readonly Mock<IConsumer<string, string>> _mockConsumer;
        private readonly Mock<ITransactionRepository> _mockTransactionRepository;
        private readonly TransactionStatusConsumer _consumer;

        public TransactionStatusConsumerTests()
        {
            _mockConsumer = new Mock<IConsumer<string, string>>();
            _mockTransactionRepository = new Mock<ITransactionRepository>();
            _consumer = new TransactionStatusConsumer(_mockTransactionRepository.Object);
            typeof(TransactionStatusConsumer)
                .GetField("_consumer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(_consumer, _mockConsumer.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldUpdateStatus_WhenMessageIsConsumed()
        {
            // Arrange
            var transactionStatus = new TransactionStatusDto
            {
                TransactionId = Guid.NewGuid(),
                Status = "COMPLETED"
            };
            var message = new Message<string, string>
            {
                Key = transactionStatus.TransactionId.ToString(),
                Value = JsonSerializer.Serialize(transactionStatus)
            };
            var consumeResult = new ConsumeResult<string, string>
            {
                Message = message
            };

            _mockConsumer.SetupSequence(c => c.Consume(It.IsAny<CancellationToken>()))
                .Returns(consumeResult)
                .Throws(new OperationCanceledException());

            _mockTransactionRepository.Setup(r => r.UpdateStatusAsync(It.IsAny<TransactionStatusDto>()))
                .ReturnsAsync(true);

            // Act
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2)))
            {
                var task = _consumer.StartAsync(cts.Token);
                await Task.Delay(500);
                cts.Cancel();
                await task;
            }

            // Assert
            _mockTransactionRepository.Verify(r => r.UpdateStatusAsync(It.Is<TransactionStatusDto>(ts =>
                ts.TransactionId == transactionStatus.TransactionId &&
                ts.Status == transactionStatus.Status
            )), Times.Once);
        }


        [Fact]
        public async Task ExecuteAsync_ShouldHandleConsumeException()
        {
            // Arrange
            _mockConsumer.Setup(c => c.Consume(It.IsAny<CancellationToken>())).Throws(new ConsumeException(new ConsumeResult<byte[], byte[]>(), new Error(ErrorCode.Local_AllBrokersDown)));

            // Act
            await _consumer.StartAsync(CancellationToken.None);

            // Assert
            _mockConsumer.Verify(c => c.Consume(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldHandleOperationCanceledException()
        {
            // Arrange
            _mockConsumer.Setup(c => c.Consume(It.IsAny<CancellationToken>())).Throws(new OperationCanceledException());

            // Act
            await _consumer.StartAsync(CancellationToken.None);

            // Assert
            _mockConsumer.Verify(c => c.Consume(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task StopAsync_ShouldCloseConsumer()
        {
            // Act
            await _consumer.StopAsync(CancellationToken.None);

            // Assert
            _mockConsumer.Verify(c => c.Close(), Times.Once);
        }
    }
}
