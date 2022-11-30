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

            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            SQLitePCL.Batteries.Init();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (!_needSeedingDb) return;

            var allMediaFiles = _seedDirectory.EnumerateFiles("*.*", SearchOption.AllDirectories)
                                              .Where(x => MediaConstants.SupportedVideoExtensions.Contains(x.Extension))
                                              .Select(y => new MediaInfoEntity
                                              {
                                                  Id = Guid.NewGuid(),
                                                  Name = Path.GetFileNameWithoutExtension(y.Name),
                                                  FullName = y.FullName,
                                                  Extension = Path.GetExtension(y.FullName),
                                                  CreationTime = y.CreationTimeUtc.ToLocalTime(),
                                                  PreviewImage = GetPreviewImageBytes(y.FullName),
                                                  LastViewedMin = 0,
                                                  IsDeleted = false
                                              })
                                              .ToList();

            modelBuilder.Entity<MediaInfoEntity>().HasData(allMediaFiles);
        }

        private static byte[] GetPreviewImageBytes(string fullName)
        {
            var outputPreviewImage = Path.Combine(AppContext.BaseDirectory, "PngResult", Guid.NewGuid() + ".png");

            var conversionTask = FFmpeg.Conversions
                                       .FromSnippet
                                       .Snapshot(fullName, outputPreviewImage, TimeSpan.FromSeconds(25));

            conversionTask.Result.Start().Wait();

            var bytes = File.ReadAllBytes(outputPreviewImage);

            if (File.Exists(outputPreviewImage))
            {
                File.Delete(outputPreviewImage);
            }

            return bytes;
        }
    }
}
