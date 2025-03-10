using Microsoft.AspNetCore.Mvc;
using Moq;
using TransactionService.Api.Controllers;
using TransactionService.Domain.DTOs;
using TransactionService.Domain.Entities;
using TransactionService.Domain.Interfaces;
using TransactionService.Domain.Shared;

namespace TransactionService.Api.Tests
{
    public class TransactionControllerTests
    {
        private readonly Mock<ITransactionService> _mockTransactionService;
        private readonly TransactionController _controller;

        public TransactionControllerTests()
        {
            _mockTransactionService = new Mock<ITransactionService>();
            _controller = new TransactionController(_mockTransactionService.Object);
        }

        [Fact]
        public async Task AddTransaction_ReturnsOkResult_WhenTransactionIsSuccessful()
        {
            // Arrange
            var transactionDto = new AddTransactionDto
            {
                Amount = 100,
                SourceAccountId = Guid.NewGuid(),
                TargetAccountId = Guid.NewGuid()
            };

            var response = new Response<Transaction>
            {
                IsSuccess = true,
                Data = new Transaction { Id = Guid.NewGuid(), Amount = 100 },
                Message = "Transaction successful",
                StatusCode = 200
            };

            _mockTransactionService.Setup(service => service.AddTransactionAsync(transactionDto))
                                   .ReturnsAsync(response);

            // Act
            var result = await _controller.AddTransaction(transactionDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Response<Transaction>>(okResult.Value);
            Assert.True(returnValue.IsSuccess);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task AddTransaction_ReturnsErrorResult_WhenTransactionFails()
        {
            // Arrange
            var transactionDto = new AddTransactionDto
            {
                Amount = 100,
                SourceAccountId = Guid.NewGuid(),
                TargetAccountId = Guid.NewGuid()
            };

            var response = new Response<Transaction>
            {
                IsSuccess = false,
                Data = null,
                Message = "Transaction failed",
                StatusCode = 500
            };

            _mockTransactionService.Setup(service => service.AddTransactionAsync(transactionDto))
                                   .ReturnsAsync(response);

            // Act
            var result = await _controller.AddTransaction(transactionDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            var returnValue = Assert.IsType<Response<Transaction>>(objectResult.Value);
            Assert.False(returnValue.IsSuccess);
            Assert.Equal(500, objectResult.StatusCode);
        }
    }
}
