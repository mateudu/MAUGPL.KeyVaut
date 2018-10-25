using System;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace MAUGPL.Web.Infrastructure.KeyVault
{
    public class KeyVaultManager : IKeyVaultManager
    {
        private readonly IConfiguration _configuration;
        private readonly Lazy<KeyVaultClient> _keyVaultClient;
        private static readonly AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();

        public KeyVaultManager(
            IConfiguration configuration
        )
        {
            _configuration = configuration;
            _keyVaultClient = GetKeyVaultClientLazy();
        }

        public async Task<string> GetSecretAsync(string key)
        {
            try
            {
                var secretUri = SecretUri(key);
                return (await _keyVaultClient.Value.GetSecretAsync(secretUri)).Value;
            }
            catch (KeyVaultErrorException exe)
            {
                throw new Exception($"Key not found: {exe}");
            }
        }

        public string SecretUri(string secret) => $"{_configuration["KeyVault:Vault"].TrimEnd('/')}/secrets/{secret}";

        private Lazy<KeyVaultClient> GetKeyVaultClientLazy() => new Lazy<KeyVaultClient>(() =>
        {
            return new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
        });

        private async Task<string> Authenticate(string authority, string resource, string scope)
        {
            var adCredential = new ClientCredential(_configuration["KeyVault:ClientId"], _configuration["KeyVault:ClientSecret"]);
            var authenticationContext = new AuthenticationContext(authority, null);
            return (await authenticationContext.AcquireTokenAsync(resource, adCredential)).AccessToken;
        }
    }
}
