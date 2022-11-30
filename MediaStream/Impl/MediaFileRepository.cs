using MediaStream.Constants;
using MediaStream.Interfaces;
using MediaStream.Interfaces.DbContext;
using MediaStream.Models;
using Microsoft.EntityFrameworkCore;

namespace MediaStream.Impl
{
    public class MediaFileRepository : IMediaFileRepository
    {
        private readonly IDbContextFactory _dbContextFactory;

        public MediaFileRepository(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public Task<string> GetFullPathByNameAsync(string fileName, CancellationToken cancellationToken) =>
            Task.Run(() =>
            {
                using (var dbContext = _dbContextFactory.CreateContext())
                {
                    var mediaInfo = dbContext.MediaInfos.FirstOrDefault(x => x.Name.Contains(fileName));

                    if (mediaInfo == null || !MediaConstants.SupportedVideoExtensions.Contains(Path.GetExtension(mediaInfo.FullName)))
                    {
                        throw new FileNotFoundException($"Matching files not found, {nameof(fileName)}: {fileName}");
                    }

                    return mediaInfo.FullName;
                }
            }, cancellationToken);

        //ToDo think about more asynchrony with IAsyncResult
        public Task<IEnumerable<MediaInfoDto>> GetAllVideoFileInfosAsync(SearchMediaFilterDto mediaFilterDto, CancellationToken cancellationToken) =>
            Task.Run(async () =>
            {
                using (var dbContext = _dbContextFactory.CreateContext())
                {
                    var query = string.IsNullOrEmpty(mediaFilterDto.FileName)
                        ? dbContext.MediaInfos.AsQueryable()
                        : dbContext.MediaInfos.Where(x => x.Name
                                                           .ToLower()
                                                           .Contains(mediaFilterDto.FileName
                                                                                   .ToLower()));

                    var mediaInfos = await query.Where(x => MediaConstants.SupportedVideoExtensions.Contains(x.Extension))
                                                .Select(y => new MediaInfoDto
                                                {
                                                    Id = y.Id,
                                                    Name = y.Name,
                                                    FullName = y.FullName,
                                                    CreationTime = y.CreationTime.ToLocalTime()
                                                                                 .ToString("MM/dd/yyyy HH:mm:ss"),
                                                    PreviewImage = y.PreviewImage
                                                })
                                                .OrderBy(x => x.FullName)
                                                .ToListAsync(cancellationToken);

                    return mediaInfos.AsEnumerable();
                }
            }, cancellationToken);
    }
}
