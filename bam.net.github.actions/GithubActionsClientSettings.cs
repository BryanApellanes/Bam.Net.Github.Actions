using Bam.Net.CoreServices.AccessControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bam.Net.Github.Actions
{
    public class GithubActionsClientSettings
    {
        public IAuthorizationHeaderProvider AuthorizationHeaderProvider { get; set; }
        public string RepoOwnerUserName { get; set; }
        public string RepoName { get; set; }

        public GithubActionsClient CreateClient()
        {
            return new GithubActionsClient(this);
        }
    }
}
