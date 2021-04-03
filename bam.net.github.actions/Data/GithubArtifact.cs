using System;
using Bam.Net.Data.Repositories;

namespace Bam.Net.Github.Actions.Artifacts.Data
{
    public class GithubArtifact : NamedAuditRepoData
    {
        public string NodeId { get; set; }
        
        public uint SizeInBytes { get; set; }
        
        public string Url { get; set; }
        
        public string ArchiveDownloadUrl { get; set; }
        
        public bool Expired { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public DateTime ExpiresAt { get; set; }
    }
}