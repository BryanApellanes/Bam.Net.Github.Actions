using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Bam.Net;
using Bam.Net.Caching;
using Bam.Net.CommandLine;
using Bam.Net.CoreServices.AccessControl;
using Bam.Net.Encryption;
using Bam.Net.Web;

namespace Bam.Net.Github.Actions
{
    public class GithubActionsClient
    {
        public GithubActionsClient()
        {
        }

        public GithubActionsClient(GithubActionsClientSettings settings) : this(settings.RepoOwnerUserName, settings.RepoName, settings.AuthorizationHeaderProvider)
        {
        }

        public GithubActionsClient(string repoOwnerUserName, string repoName, IAuthorizationHeaderProvider authorizationHeaderProvider = null)
        {
            RepoOwnerUserName = repoOwnerUserName;
            RepoName = repoName;
            AuthorizationHeaderProvider = authorizationHeaderProvider ?? new EnvironmentVariableAuthorizationHeaderProvider("GithubAccessToken");
            Cache = Cache<GithubArtifactInfo>.Get();
        }

        public virtual IEnumerable<GithubArtifactInfo> ListArtifactInfos(string repoOwnerUserName = null, string repoName = null)
        {
            repoOwnerUserName ??= RepoOwnerUserName;
            repoName ??= RepoName;
            GithubArtifactsGetResponse response = Http.GetJson<GithubArtifactsGetResponse>(GetArtifactsUri(repoOwnerUserName, repoName).ToString(), GetHeaders(true));
            foreach (GithubArtifactInfo artifact in response.Artifacts)
            {
                artifact.AuthorizationHeaderProvider = AuthorizationHeaderProvider;
                yield return artifact;
            }
        }

        public virtual GithubArtifactInfo GetArtifactInfo(uint artifactId, string repoOwnerUserName = null, string repoName = null)
        {
            GithubArtifactInfo artifactInfo = Http.GetJson<GithubArtifactInfo>(GetApiUri(artifactId.ToString(), repoOwnerUserName, repoName).ToString(), GetHeaders(true));
            artifactInfo.AuthorizationHeaderProvider = AuthorizationHeaderProvider;
            return artifactInfo;
        }

        public virtual bool DeleteArtifact(uint artifactId, string repoOwnerUserName = null, string repoName = null)
        {
            HttpResponseMessage responseMessage = Http.DeleteAsync(GetApiUri(artifactId.ToString(), repoOwnerUserName, repoName).ToString(), GetHeaders(true)).Result;
            return responseMessage.IsSuccessStatusCode;
        }

        protected Cache<GithubArtifactInfo> Cache
        {
            get;
            set;
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