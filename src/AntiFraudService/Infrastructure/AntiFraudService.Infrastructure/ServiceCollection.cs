using AntiFraudService.Infrastructure.Kafka;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntiFraudService.Infrastructure
{
  public static class ServiceCollection
  {
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
      services.AddHostedService<AntiFraudConsumer>();

      return services;
    }
  }
}
