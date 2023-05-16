using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace ProyectoVirtualStore.Helpers
{
    public static class HelperKeyVault
    {
        public static async Task<string> GetSecretValue(string secretName)
        {
            
            string keyVaultUrl = $"https://keyvaulttiendajpv.vault.azure.net/";

            var credential = new DefaultAzureCredential();
            var secretClient = new SecretClient(new Uri(keyVaultUrl), credential);

            KeyVaultSecret secret = await secretClient.GetSecretAsync(secretName);
            return secret.Value;
        }
    }
}
