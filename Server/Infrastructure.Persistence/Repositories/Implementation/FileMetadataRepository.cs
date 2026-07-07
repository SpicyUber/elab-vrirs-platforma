using Domain.Entities;
using Infrastructure.Persistence.Repositories.Interfaces;

namespace Infrastructure.Persistence.Repositories.Implementation
{
    public class FileMetadataRepository : GenericRepository<FileMetadata>, IFileMetadataRepository
    {
        public FileMetadataRepository(VrirsDbContext context) : base(context)
        {
        }
    }
}
