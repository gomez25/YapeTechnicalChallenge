using Microsoft.EntityFrameworkCore;
using Moq;
using TransactionService.Domain.DTOs;
using TransactionService.Domain.Entities;
using TransactionService.Domain.Exceptions;
using TransactionService.Domain.Interfaces;
using TransactionService.Infrastructure.Database;

namespace TransactionService.Infrastructure.Tests
{
    public class TransactionRepositoryTests
    {
        private readonly Mock<ITransactionRepository> _mockRepository;
        private readonly ITransactionRepository _repository;

        public TransactionRepositoryTests()
        {
            _mockRepository = new Mock<ITransactionRepository>();
            _repository = _mockRepository.Object;
        }

        [Fact]
        public async Task CreateTransactionAsync_ShouldAddTransaction()
        {
            // Arrange
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Amount = 100,
                SourceAccountId = Guid.NewGuid(),
                TargetAccountId = Guid.NewGuid(),
                Status = "PENDING",
                CreatedAt = DateTime.UtcNow
            };

            _mockRepository.Setup(repo => repo.CreateTransactionAsync(transaction)).ReturnsAsync(transaction);

            // Act
            var result = await _repository.CreateTransactionAsync(transaction);

            // Assert
            _mockRepository.Verify(repo => repo.CreateTransactionAsync(transaction), Times.Once);
            Assert.Equal(transaction, result);
        }

        [Fact]
        public async Task GetTransactionWithTotalAmountAsync_ShouldReturnTransactionDto()
        {
            // Arrange
            var transactionSourceId = Guid.NewGuid();
            var transactionDto = new TransactionDto
            {
                Id = Guid.NewGuid(),
                Amount = 100,
                TotalAmount = 200,
                Status = "COMPLETED"
            };

            _mockRepository.Setup(repo => repo.GetTransactionWithTotalAmountAsync(transactionSourceId)).ReturnsAsync(transactionDto);

            // Act
            var result = await _repository.GetTransactionWithTotalAmountAsync(transactionSourceId);

            // Assert
            _mockRepository.Verify(repo => repo.GetTransactionWithTotalAmountAsync(transactionSourceId), Times.Once);
            Assert.Equal(transactionDto, result);
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldUpdateTransactionStatus()
        {
            // Arrange
            var transactionStatusDto = new TransactionStatusDto
            {
                TransactionId = Guid.NewGuid(),
                Status = "COMPLETED"
            };

            _mockRepository.Setup(repo => repo.UpdateStatusAsync(transactionStatusDto)).ReturnsAsync(true);

            // Act
            var result = await _repository.UpdateStatusAsync(transactionStatusDto);

            // Assert
            _mockRepository.Verify(repo => repo.UpdateStatusAsync(transactionStatusDto), Times.Once);
            Assert.True(result);
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldThrowTransactionNotFound()
        {
            // Arrange
            var transactionStatusDto = new TransactionStatusDto
            {
                TransactionId = Guid.NewGuid(),
                Status = "COMPLETED"
            };

            _mockRepository.Setup(repo => repo.UpdateStatusAsync(transactionStatusDto)).ThrowsAsync(new TransactionNotFound());

            // Act & Assert
            await Assert.ThrowsAsync<TransactionNotFound>(() => _repository.UpdateStatusAsync(transactionStatusDto));
        }
    }
}
