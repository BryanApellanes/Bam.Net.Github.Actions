using System;
using System.Collections.Generic;
using System.Net.Http;
using Bam.Net;
using Bam.Net.CommandLine;
using Bam.Net.CoreServices.AccessControl;
using Bam.Net.Encryption;
using Bam.Net.Web;

namespace Bam.Net.Github.Actions
{
    public class GithubActionsClient
    {
        public GithubActionsClient(string repoOwnerUserName = null, string repoName = null) : this(new GithubVaultAuthorizationHeaderProvider())
        {
            RepoOwnerUserName = repoOwnerUserName;
            RepoName = repoName;
        }

        public GithubActionsClient(Vault vault,string repoOwnerUserName = null, string repoName = null) : this(new GithubVaultAuthorizationHeaderProvider(vault))
        {
            RepoOwnerUserName = repoOwnerUserName;
            RepoName = repoName;
        }

        public GithubActionsClient(IAuthorizationHeaderProvider authorizationHeaderProvider)
        {
            AuthorizationHeaderProvider = authorizationHeaderProvider;
        }

        public virtual IEnumerable<GithubArtifact> GetArtifacts(string repoOwnerUserName = null, string repoName = null)
        {
            repoOwnerUserName ??= RepoOwnerUserName;
            repoName ??= RepoName;
            GithubArtifactsGetResponse response = Http.GetJson<GithubArtifactsGetResponse>(GetArtifactsUri(repoOwnerUserName, repoName).ToString(), GetHeaders(true));
            return response.Artifacts;
        }

        public virtual GithubArtifact GetArtifact(uint artifactId, string repoOwnerUserName = null, string repoName = null)
        {
            return Http.GetJson<GithubArtifact>(GetApiUri(artifactId.ToString(), repoOwnerUserName, repoName).ToString(), GetHeaders(true));
        }

        public virtual bool DeleteArtifact(uint artifactId, string repoOwnerUserName = null, string repoName = null)
        {
            HttpResponseMessage responseMessage = Http.DeleteAsync(GetApiUri(artifactId.ToString(), repoOwnerUserName, repoName).ToString(), GetHeaders(true)).Result;
            return responseMessage.IsSuccessStatusCode;
        }
        
        public IAuthorizationHeaderProvider AuthorizationHeaderProvider
        {
            get;
            private set;
        }
        
        public string RepoOwnerUserName { get; set; }
        public string RepoName { get; set; }

        protected Uri GetApiUri(string path, string repoOwnerUserName = null, string repoName = null)
        {
            if (!path.StartsWith("/"))
            {
                path = $"/{path}";
            }
            return new Uri($"{GetArtifactsUri().ToString()}{path}");
        }
        
        protected Uri GetArtifactsUri(string repoOwnerUserName = null, string repoName = null)
        {
            repoOwnerUserName ??= RepoOwnerUserName;
            repoName ??= RepoName;
            return new Uri($"{GetGithubApiDomain()}{GetArtifactsPath(repoOwnerUserName, repoName)}");
        }
        
        protected virtual string GetGithubApiDomain()
        {
            return "https://api.github.com";
        }
        
        protected string GetRepoPath(string repoOwnerUserName = null, string repoName = null)
        {
            repoOwnerUserName ??= RepoOwnerUserName;
            repoName ??= RepoName;
            return $"/repos/{repoOwnerUserName}/{repoName}";
        }

        protected string GetArtifactsPath(string repoOwnerUserName = null, string repoName = null)
        {
            repoOwnerUserName ??= RepoOwnerUserName;
            repoName ??= RepoName;
            return $"{GetRepoPath(repoOwnerUserName, repoName)}/actions/artifacts";
        }
        
        protected virtual string GetGithubToken()
        {
            string key = "GithubToken";
            Config config = Config.Current;
            string githubToken = config[key, null];
            if (string.IsNullOrEmpty(githubToken))
            {
                string msgSignature = "{0} was not found in config file ({1})";
                Message.PrintLine(msgSignature, ConsoleColor.DarkRed, key, config.File.FullName);
                Args.Throw<InvalidOperationException>(msgSignature, key, config.File.FullName);
            }

            return githubToken;
        }

        protected Dictionary<string, string> GetHeaders(bool includeAuthorizationHeader = false)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (includeAuthorizationHeader)
            {
                AuthorizationHeader header = AuthorizationHeaderProvider.GetAuthorizationHeader();
                header.Add(result);
            }
            return result;
        }
    }
}