using MediaStream.Interfaces.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MediaStream.Impl.DbContext
{
    public class DbContextFactory : IDbContextFactory
    {
        private readonly AppSettings _appSettings;

        public DbContextFactory(IOptions<AppSettings> options)
        {
            _appSettings = options.Value;
        }

        public IDbContext CreateContext()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder();
            dbContextOptionsBuilder.UseSqlite($"Filename={Path.Combine(AppContext.BaseDirectory, $"{_appSettings.DbFileName}.db")}");

            return new DbContext(dbContextOptionsBuilder.Options, _appSettings.SearchDirectory, _appSettings.NeedSeedingDb);
        }
    }
}
