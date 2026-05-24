using Domain.Enums;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class FileMetadata : IAuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string StoragePath { get; set; } = string.Empty;
        public string Mime { get; set; } = string.Empty;

        public string Extension { get; set; } = string.Empty;
        public long SizeInBytes { get; set; }

        public FileStatus Status { get; set; } = FileStatus.Uploading;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Guid UploadedByUserId { get; set; }
        public User UploadedByUser { get; set; } = null!;
    }
}
