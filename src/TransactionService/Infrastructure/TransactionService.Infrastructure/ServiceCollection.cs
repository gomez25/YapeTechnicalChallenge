using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TransactionService.Domain.Interfaces;
using TransactionService.Infrastructure.Database;
using TransactionService.Infrastructure.Kafka;
using TransactionService.Infrastructure.Repositories;

namespace TransactionService.Infrastructure
{
  public static class ServiceCollection
  {
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string connectionString)
    {
      services.AddDbContext<TransactionContext>(options => options.UseNpgsql(connectionString));
      services.AddSingleton<IKafkaProducer, TransactionServiceProducer>();
      services.AddScoped<ITransactionRepository, TransactionRepository>();
      services.AddHostedService<TransactionStatusConsumer>();

      return services;
    }
  }
}
