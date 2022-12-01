using MediaStream.Constants;
using MediaStream.Impl.DbContext.Entities;
using MediaStream.Interfaces.DbContext;
using Microsoft.EntityFrameworkCore;
using Xabe.FFmpeg;

namespace MediaStream.Impl.DbContext
{
    public sealed class DbContext : Microsoft.EntityFrameworkCore.DbContext, IDbContext
    {
        private readonly DirectoryInfo _seedDirectory;
        private readonly bool _needSeedingDb;

        public DbSet<MediaInfoEntity> MediaInfos { get; set; }

        public DbContext(DbContextOptions dBContextOptions, 
                         string mediaDirectory, 
                         bool needSeedingDb = false) : base(dBContextOptions)
        {
            _seedDirectory = new DirectoryInfo(mediaDirectory);
            _needSeedingDb = needSeedingDb;

            if (_needSeedingDb)
            {
                Database.EnsureDeleted();
            }
            
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            SQLitePCL.Batteries.Init();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (!_needSeedingDb) return;

            Task.Run(() => SeedDbAsync(modelBuilder)).Wait();
        }

        private async Task SeedDbAsync(ModelBuilder modelBuilder)
        {
            var allMediaFiles = _seedDirectory.EnumerateFiles("*.*", SearchOption.AllDirectories)
                                              .Where(x => MediaConstants.SupportedVideoExtensions.Contains(x.Extension))
                                              .Select(async y =>
                                              {
                                                  var id = Guid.NewGuid();
                                                  var theme = y.Directory!.Name;
                                                  var extension = Path.GetExtension(y.FullName);
                                                  var name = Path.GetFileNameWithoutExtension(y.Name);

                                                  return new MediaInfoEntity
                                                  {
                                                      Id = id,
                                                      Name = name,
                                                      Theme = theme,
                                                      LastViewedMin = 0,
                                                      IsDeleted = false,
                                                      FullName = y.FullName,
                                                      Extension = extension,
                                                      CreationTime = y.CreationTimeUtc.ToLocalTime(),
                                                      PreviewImage = await GetPreviewImageBytesAsync(y.FullName),
                                                      FullSearchText = $"{id}; {name}; {y.FullName}; {theme}".ToLower(),
                                                  };
                                              })
                                              .ToList();

            await Task.WhenAll(allMediaFiles);

            modelBuilder.Entity<MediaInfoEntity>().HasData(allMediaFiles.Select(x => x.Result).ToList());
        }

        private static async Task<byte[]> GetPreviewImageBytesAsync(string fullName)
        {
            var outputPreviewImage = Path.Combine(AppContext.BaseDirectory, "PngResult", Guid.NewGuid() + ".png");

            var conversion = await FFmpeg.Conversions
                                         .FromSnippet
                                         .Snapshot(fullName, outputPreviewImage, TimeSpan.FromSeconds(25));

            await conversion.Start();

            var bytes = await File.ReadAllBytesAsync(outputPreviewImage);

            if (File.Exists(outputPreviewImage))
            {
                File.Delete(outputPreviewImage);
            }

            return bytes;
        }
    }
}
