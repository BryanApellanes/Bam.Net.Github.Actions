using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Bam.Net.Caching;
using Bam.Net.CoreServices.AccessControl;
using Bam.Net.Web;
using Newtonsoft.Json;

namespace Bam.Net.Github.Actions
{
    [Serializable]
    public class GithubArtifactInfo : IMemorySize
    {
        [JsonProperty("id")]
        public uint Id { get; set; }
        
        [JsonProperty("node_id")]
        public string NodeId { get; set; }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("size_in_bytes")]
        public uint SizeInBytes { get; set; }
        
        [JsonProperty("url")]
        public string Url { get; set; }
        
        [JsonProperty("archive_download_url")]
        public string ArchiveDownloadUrl { get; set; }
        
        [JsonProperty("expired")]
        public bool Expired { get; set; }
        
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
        
        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
        
        [JsonProperty("expires_at")]
        public DateTime ExpiresAt { get; set; }

        [JsonIgnore]
        public IAuthorizationHeaderProvider AuthorizationHeaderProvider
        {
            get;
            set;
        }
        
        public FileInfo Download()
        {
            return DownloadTo(new FileInfo($"./{Name}.zip"));
        }

        public FileInfo DownloadTo(FileInfo fileInfo)
        {
            return DownloadTo(fileInfo.FullName);
        }

        public FileInfo DownloadTo(string filePath)
        {
            byte[] fileData = Http.GetData($"{ArchiveDownloadUrl}", new System.Collections.Generic.Dictionary<string, string>()
            {
                { "Authorization", $"token {AuthorizationHeaderProvider.GetRawValue()}"}
            });
            File.WriteAllBytes(filePath, fileData);
            return new FileInfo(filePath);
        }

        public int MemorySize()
        {
            return this.ToBinaryBytes().Length;
        }
    }
}