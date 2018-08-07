using Sitecore.Abstractions;

namespace Sitecore.Configuration
{
  public class AzureKeyVaultSettings : DefaultSettings
  {
    public AzureKeyVaultSettings(BaseFactory factory, BaseLog log) : base(factory, log)
    {
    }

    public override string GetConnectionString(string connectionStringName)
    {
      return base.GetConnectionString(connectionStringName);
    }
  }
}