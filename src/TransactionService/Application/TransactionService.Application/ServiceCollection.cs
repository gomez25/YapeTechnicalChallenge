using Microsoft.Extensions.DependencyInjection;
using TransactionService.Domain.Interfaces;

namespace TransactionService.Application
{
    public static class ServiceCollection
  {
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
      services.AddScoped<ITransactionService, TransactionService>();
      return services;
    }
  }
}
