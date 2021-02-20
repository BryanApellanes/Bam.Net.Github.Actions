using Bam.Net.CoreServices.AccessControl;
using Bam.Net.Encryption;
using EcmaScript.NET;

namespace Bam.Net.Github.Actions
{
    public class GithubVaultAuthorizationHeaderProvider : VaultAuthorizationHeaderProvider
    {
        public GithubVaultAuthorizationHeaderProvider()
        {
            ConfigKey = "GithubAuthorizationHeader";
            TokenType = TokenTypes.Token;
            Vault = Vault.Profile;
        }

        public GithubVaultAuthorizationHeaderProvider(Vault vault) : this()
        {
            Vault = vault;
        }
    }
}