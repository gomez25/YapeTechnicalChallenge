using Microsoft.EntityFrameworkCore;
using TransactionService.Domain.Entities;

namespace TransactionService.Infrastructure.Database
{
    public class TransactionContext(DbContextOptions<TransactionContext> options) : DbContext(options)
    {
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<TransferType> TransferTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("accounts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255);
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("transactions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Amount).HasColumnName("amount");
                entity.Property(e => e.SourceAccountId).HasColumnName("source_account_id");
                entity.Property(e => e.TargetAccountId).HasColumnName("target_account_id");
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.TransferTypeId).HasColumnName("transfer_type_id");

                entity.HasOne(t => t.SourceAccount)
                      .WithMany(a => a.SourceTransactions)
                      .HasForeignKey(t => t.SourceAccountId);

                entity.HasOne(t => t.TargetAccount)
                      .WithMany(a => a.TargetTransactions)
                      .HasForeignKey(t => t.TargetAccountId);

                entity.HasOne(t => t.TransferType)
                      .WithMany(tt => tt.Transactions)
                      .HasForeignKey(t => t.TransferTypeId);
            });

            modelBuilder.Entity<TransferType>(entity =>
            {
                entity.ToTable("transfer_types");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Type).HasColumnName("type").HasMaxLength(100);
            });
        }

        public void MigrateDatabase()
        {
            Database.Migrate();
        }
    }
}
