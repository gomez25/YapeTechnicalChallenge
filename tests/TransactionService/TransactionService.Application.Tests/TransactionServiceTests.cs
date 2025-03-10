using Moq;
using TransactionService.Domain.DTOs;
using TransactionService.Domain.Entities;
using TransactionService.Domain.Interfaces;

namespace TransactionService.Application.Tests
{
    public class TransactionServiceTests
    {
        private readonly Mock<IKafkaProducer> _mockKafkaProducer;
        private readonly Mock<ITransactionRepository> _mockTransactionRepository;
        private readonly TransactionService _transactionService;

        public TransactionServiceTests()
        {
            _mockKafkaProducer = new Mock<IKafkaProducer>();
            _mockTransactionRepository = new Mock<ITransactionRepository>();
            _transactionService = new TransactionService(_mockKafkaProducer.Object, _mockTransactionRepository.Object);
        }

        [Fact]
        public async Task AddTransactionAsync_ReturnsSuccessResponse_WhenTransactionIsProcessed()
        {
            // Arrange
            var transactionDto = new AddTransactionDto
            {
                Amount = 100,
                SourceAccountId = Guid.NewGuid(),
                TargetAccountId = Guid.NewGuid()
            };

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                SourceAccountId = transactionDto.SourceAccountId,
                TargetAccountId = transactionDto.TargetAccountId,
                Amount = transactionDto.Amount,
                Status = "PENDING",
                CreatedAt = DateTime.UtcNow
            };

            var transactionWithTotalAmount = new TransactionDto
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                TotalAmount = 1000,
                Status = "PENDING"
            };

            _mockTransactionRepository.Setup(repo => repo.CreateTransactionAsync(It.IsAny<Transaction>()))
                                      .ReturnsAsync(transaction);

            _mockTransactionRepository.Setup(repo => repo.GetTransactionWithTotalAmountAsync(transactionDto.SourceAccountId))
                                      .ReturnsAsync(transactionWithTotalAmount);

            _mockKafkaProducer.Setup(producer => producer.PublishTransactionAsync(It.IsAny<TransactionDto>()))
                              .Returns(Task.CompletedTask);

            // Act
            var result = await _transactionService.AddTransactionAsync(transactionDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(transaction, result.Data, new TransactionComparer());
            _mockTransactionRepository.Verify(repo => repo.CreateTransactionAsync(It.IsAny<Transaction>()), Times.Once);
            _mockTransactionRepository.Verify(repo => repo.GetTransactionWithTotalAmountAsync(transactionDto.SourceAccountId), Times.Once);
            _mockKafkaProducer.Verify(producer => producer.PublishTransactionAsync(It.IsAny<TransactionDto>()), Times.Once);
        }
    }
}
