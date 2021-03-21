using System;
using Newtonsoft.Json;

namespace Bam.Net.Github.Actions
{
    [Serializable]
    public class GithubArtifactsGetResponse
    {
        [JsonProperty("total_count")]
        public int TotalCount { get; set; }
        
        [JsonProperty("artifacts")]
        public GithubArtifactInfo[] Artifacts { get; set; }
    }
}