﻿using System;
using System.Collections.Generic;
using Bam.Net;
using Bam.Net.CommandLine;
using Bam.Net.Web;

namespace Bam.Net.Github.Actions
{
    public class GithubActionsClient
    {
        public virtual IEnumerable<GithubArtifact> GetArtifacts(string repoOwnerUserName, string repoName)
        {
            GithubArtifactsGetResponse response = Http.GetJson<GithubArtifactsGetResponse>(GetArtifactsUri(repoOwnerUserName, repoName).ToString(), GetHeaders(true));
            return response.Artifacts;
        }

        protected Uri GetArtifactsUri(string repoOwnerUserName, string repoName)
        {
            return new Uri($"{GetGithubApiDomain()}{GetArtifactsPath(repoOwnerUserName, repoName)}");
        }
        
        protected string GetGithubApiDomain()
        {
            return "https://api.github.com";
        }
        
        protected string GetRepoPath(string repoOwnerUserName, string repoName)
        {
            return $"/repos/{repoOwnerUserName}/{repoName}";
        }

        protected string GetArtifactsPath(string repoOwnerUserName, string repoName)
        {
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
                result.Add("Authorization", GetGithubToken());
            }
            return result;
        }
    }
}