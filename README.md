# Sitecore Azure KeyVault

This repository contains a POC of how to integrate Azure Key Vault with Sitecore.

## Limitations

+ Works only with connection strings for content databases `core`, `master` and `web`.
+ Doesn't support connection strings for a Session State Provider.
+ Doesn't support connection strings for xDB and/or xConnect.
+ Doesn't work with ASP.NET Membership, which by default sits inside the database `core`.

## Requirements

It is very easy to get started with Azure Key Vault and integrate it with existent ASP.NET Web Application. You just need the following components.

- A work or school account / Microsoft account and a Microsoft Azure subscription.
- Azure CLI
- Sitecore® Experience Platform™ 9.0

## Instructions

Let's say there is a running Sitecore solution, which you want to read connection strings from Azure Key Vault for Sitecore databases. First, you need to create Azure Key Vault, and add secrets with connection strings. Then add app

I'm going to use Azure CLI, but you can use whatever works best for you - Azure Portal, PowerShell, Shell, CLI etc.  

> NOTE: To run the following commands from a local machine, you need to log in first and then switch to a right Azure Subscription.

```console
az login
```

```console
az account set -s Visual Studio Premium with MSDN
```

1. Create a new Azure Key Vault.

```console
az keyvault create -n Sitecore -g Oleg-Burov
```

2. Add a secret to the Key Vault.

```console
az keyvault secret set -n web --vault-name Sitecore --value 'Server=tcp:sitecore.database.windows.net,1433;Initial Catalog=web-db;Persist Security Info=False;User ID=sa;Password=12345;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
```

> NOTE: Use connection string's name as a secret's name.

3. Create a new App registration within Azure Active Directory (AD).

```console
az ad app create --display-name SitecoreAzureKeyVault --identifier-uris 'https://keyvault.sitecore.com' --password 12345
```

4. Authorize the application to read secrets in the Key Vault. 

```console
az keyvault set-policy -n Sitecore --spn 76222698-83f4-4d4c-a815-ddff0b7f1482 --secret-permissions get
```

> NOTE: To get application's ID use the following command:

```console
az ad app list --display-name SitecoreAzureKeyVault --query [].appId
```

5. [Download](../../releases) and unzip the package into a folder with Sitecore installation.

6. Update the configuration file `\App_Config\ConnectionStrings.config` with secret identifier from Key Vault.

```
<add name="core" connectionString="https://sitecore.vault.azure.net/secrets/core/c143d13b5a5c47f49de2e09e52d27f35" />
<add name="master" connectionString="https://sitecore.vault.azure.net/secrets/master/7b00c9fbbfc84f3fab71374f677d05e3" />
<add name="web" connectionString="https://sitecore.vault.azure.net/secrets/web/2e00fbcac52a4c04beccac9007110052" /> 
```

> NOTE: To get a secrete identifier use the following command.

```console
az keyvault secret show --vault-name Sitecore -n core --query id
```

7. Update the configuration file `Web.config` with following two app settings.

```xml
<add key="ClientId" value="76222698-83f4-4d4c-a815-ddff0b7f1482" />
<add key="ClientSecret" value="12345" />
```

> NOTE: ClientId is App registration's ID in Azure AD, which you created in step #3. To get application's ID use the following command.

```console
az ad app list --display-name SitecoreAzureKeyVault --query [].appId
```

8. Move ASP.NET Membership from the database `core` to a separate one, for example `security`, and then update the configuration files `Web.config`.

```xml
<membership>
  <providers>
  ...
    <add name="sql" type="System.Web.Security.SqlMembershipProvider" connectionStringName="security" ... />
  ...
  </providers>
</membership>

<roleManager>
  <providers>
  ...
    <add name="sql" type="System.Web.Security.SqlRoleProvider" connectionStringName="security" ... />
  ...
  </providers>
</roleManager>

<profile>
  <providers>
  ...
    <add name="sql" type="System.Web.Profile.SqlProfileProvider" connectionStringName="security" ... />
  ...
  </providers>
</profile>
```
