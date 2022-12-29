using MediaStream.Interfaces.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MediaStream.Impl.DbContext
{
    public class DbContextFactory : IDbContextFactory
    {
        private readonly IOptions<AppSettings> _options;

        public DbContextFactory(IOptions<AppSettings> options) =>
            _options = options;

        public IDbContext CreateContext()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder();
            dbContextOptionsBuilder.UseSqlite($"Filename={Path.Combine(AppContext.BaseDirectory, $"{_options.Value.DbFileName}.db")}");

            return new DbContext(dbContextOptionsBuilder.Options, _options);
        }
    }
}
