using MediaStream.Impl.DbContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediaStream.Interfaces.DbContext
{
    public interface IDbContext : IDisposable
    {
        public DbSet<MediaInfoEntity> MediaInfos { get; set; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
