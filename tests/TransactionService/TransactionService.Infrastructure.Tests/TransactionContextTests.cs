using Microsoft.EntityFrameworkCore;
using TransactionService.Domain.Entities;
using TransactionService.Infrastructure.Database;

namespace TransactionService.Infrastructure.Tests
{
    public class TransactionContextTests
    {
        private DbContextOptions<TransactionContext> CreateInMemoryOptions()
        {
            return new DbContextOptionsBuilder<TransactionContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void CanInsertTransactionIntoDatabase()
        {
            var options = CreateInMemoryOptions();

            using (var context = new TransactionContext(options))
            {
                var transaction = new Transaction
                {
                    Id = Guid.NewGuid(),
                    SourceAccountId = Guid.NewGuid(),
                    TargetAccountId = Guid.NewGuid(),
                    Amount = 100,
                    Status = "PENDING",
                    CreatedAt = DateTime.UtcNow,
                    TransferTypeId = 1
                };

                context.Transactions.Add(transaction);
                context.SaveChanges();
            }

            using (var context = new TransactionContext(options))
            {
                Assert.Equal(1, context.Transactions.Count());
                var transaction = context.Transactions.Single();
                Assert.Equal(100, transaction.Amount);
                Assert.Equal("PENDING", transaction.Status);
            }
        }

        [Fact]
        public void CanInsertAccountIntoDatabase()
        {
            var options = CreateInMemoryOptions();

            using (var context = new TransactionContext(options))
            {
                var account = new Account
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Account"
                };

                context.Accounts.Add(account);
                context.SaveChanges();
            }

            using (var context = new TransactionContext(options))
            {
                Assert.Equal(1, context.Accounts.Count());
                var account = context.Accounts.Single();
                Assert.Equal("Test Account", account.Name);
            }
        }

        [Fact]
        public void CanInsertTransferTypeIntoDatabase()
        {
            var options = CreateInMemoryOptions();

            using (var context = new TransactionContext(options))
            {
                var transferType = new TransferType
                {
                    Id = 1,
                    Type = "Test Transfer Type"
                };

                context.TransferTypes.Add(transferType);
                context.SaveChanges();
            }

            using (var context = new TransactionContext(options))
            {
                Assert.Equal(1, context.TransferTypes.Count());
                var transferType = context.TransferTypes.Single();
                Assert.Equal("Test Transfer Type", transferType.Type);
            }
        }
    }
}
