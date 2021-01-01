using System;
using System.IO;
using Bam.Net.Web;
using Newtonsoft.Json;

namespace Bam.Net.Github.Actions
{
    [Serializable]
    public class GithubArtifact
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

        public FileInfo DownloadTo(FileInfo fileInfo)
        {
            return DownloadTo(fileInfo.FullName);
        }

        public FileInfo DownloadTo(string filePath)
        {
            byte[] fileData = Http.GetData(ArchiveDownloadUrl);
            File.WriteAllBytes(filePath, fileData);
            return new FileInfo(filePath);
        }
    }
}