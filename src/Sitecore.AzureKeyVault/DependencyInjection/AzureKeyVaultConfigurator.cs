using Microsoft.Extensions.DependencyInjection;
using Sitecore.Abstractions;
using Sitecore.Configuration;
using System.Linq;

namespace Sitecore.DependencyInjection
{
  public class AzureKeyVaultConfigurator : IServicesConfigurator
  {
    public void Configure(IServiceCollection serviceCollection)
    {
      var serviceDescriptor = serviceCollection.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(BaseSettings));
      _ = serviceCollection.Remove(serviceDescriptor);
      serviceCollection.AddSingleton(typeof(BaseSettings), typeof(AzureKeyVaultSettings));
    }
  }
}