using System.Threading.Tasks;

namespace MAUGPL.Web.Infrastructure.KeyVault
{
    public interface IKeyVaultManager
    {
        Task<string> GetSecretAsync(string key);
    }
}
