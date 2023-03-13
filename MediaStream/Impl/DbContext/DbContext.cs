using MediaStream.Constants;
using MediaStream.Impl.DbContext.Entities;
using MediaStream.Interfaces.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Xabe.FFmpeg;

namespace MediaStream.Impl.DbContext
{
    public sealed class DbContext : Microsoft.EntityFrameworkCore.DbContext, IDbContext
    {
        private static DirectoryInfo? _seedDirectory;
        private static bool _needSeedingDb;

        public DbSet<MediaInfoEntity> MediaInfos { get; set; }

        public DbContext(DbContextOptions dBContextOptions, 
                         IOptions<AppSettings> options) : base(dBContextOptions) =>
            ReloadDbIfNeeded(options);

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            SQLitePCL.Batteries.Init();

        protected override void OnModelCreating(ModelBuilder modelBuilder) =>
            Task.Run(() => SeedDbIfNeededAsync(modelBuilder)).Wait();

        private static async Task SeedDbIfNeededAsync(ModelBuilder modelBuilder)
        {
            if (!_needSeedingDb) return;

            var allMediaFiles = _seedDirectory?.EnumerateFiles("*.*", SearchOption.AllDirectories)
                                              .Where(x => MediaConstants.SupportedVideoExtensions.Contains(x.Extension))
                                              .Select(async y =>
                                              {
                                                  var id = Guid.NewGuid();
                                                  var theme = y.Directory!.Name;
                                                  var extension = Path.GetExtension(y.FullName);
                                                  var name = Path.GetFileNameWithoutExtension(y.Name);
                                                  var output = Path.Combine(y.DirectoryName!, Path.GetFileNameWithoutExtension(name) + ".mp4");

                                                  if (extension == ".mkv")
                                                  {
                                                      //var command = $"-i {y.FullName} -c:v libx264 -crf 16 -preset slow -c:a aac -b:a 192k -ac 2 {output}";
                                                      //var command = $"-i {y.FullName} -c:v libx264 -crf 20 -c:a aac {output}";
                                                      //var command = $"-i {y.FullName} -vcodec copy -codec:a libmp3lame -qscale:a 2 {output}";
                                                      var command = $"-i {y.FullName} -vcodec copy -c:a aac -b:a 192k -ac 2 {output}";
                                                      var conversion = FFmpeg.Conversions.New();
                                                      await conversion.AddParameter(command)
                                                                      .Start();
                                                  }

                                                  return new MediaInfoEntity
                                                  {
                                                      Id = id,
                                                      Name = name,
                                                      Theme = theme,
                                                      IsLiked = false,
                                                      LastViewedMin = 0,
                                                      IsDeleted = false,
                                                      Extension = extension,
                                                      CreationTime = y.CreationTimeUtc.ToLocalTime(),
                                                      FullName = File.Exists(output) ? output : y.FullName,
                                                      PreviewImage = await GetPreviewImageBytesAsync(y.FullName),
                                                      FullSearchText = $"{id}; {name}; {y.FullName}; {theme}".ToLower(),
                                                  };
                                              })
                                              .ToList();

            //var allMediaFiles = _seedDirectory?.EnumerateFiles("*.*", SearchOption.AllDirectories)
            //                      .Where(x => MediaConstants.SupportedVideoExtensions.Contains(x.Extension))
            //                      .ToList();

            allMediaFiles ??= new List<Task<MediaInfoEntity>>();

            await Task.WhenAll(allMediaFiles);

            modelBuilder.Entity<MediaInfoEntity>().HasData(allMediaFiles.Select(x => x.Result).ToList());

            _needSeedingDb = false;
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

        private void ReloadDbIfNeeded(IOptions<AppSettings> options)
        {
            if (options.Value.NeedSeedingDb)
            {
                Database.EnsureDeleted();

                _needSeedingDb = true;
                _seedDirectory = Directory.Exists(options.Value.SearchDirectory) 
                    ? new DirectoryInfo(options.Value.SearchDirectory) 
                    : throw new DirectoryNotFoundException($"Directory {nameof(options.Value.SearchDirectory)} not found. Full path: {options.Value.SearchDirectory}"); ;

                options.Value.NeedSeedingDb = false;
            }

            Database.EnsureCreated();
        }
    }
}
