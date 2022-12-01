using System.Runtime.CompilerServices;
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

        public async Task<string> GetFullPathByNameAsync(string fileName, CancellationToken cancellationToken)
        {
            using (var dbContext = _dbContextFactory.CreateContext())
            {
                var mediaInfo = await dbContext.MediaInfos.FirstOrDefaultAsync(x => x.Name.Contains(fileName), cancellationToken);

                if (mediaInfo == null || !MediaConstants.SupportedVideoExtensions.Contains(Path.GetExtension(mediaInfo.FullName)))
                {
                    throw new FileNotFoundException($"Matching files not found, {nameof(fileName)}: {fileName}");
                }

                return mediaInfo.FullName;
            }
        }

        public async IAsyncEnumerable<MediaInfoDto> GetAllVideoFileInfosAsync(SearchMediaFilterDto mediaFilterDto, 
                                                                             [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            using (var dbContext = _dbContextFactory.CreateContext())
            {
                var fileName = mediaFilterDto.FileName?.ToLower();

                var query = string.IsNullOrEmpty(mediaFilterDto.FileName)
                    ? dbContext.MediaInfos
                    : dbContext.MediaInfos.Where(x => x.Name.ToLower().Contains(fileName!));
                
                foreach (var mediaInfo in await query.Where(x => MediaConstants.SupportedVideoExtensions.Contains(x.Extension))
                                                     .OrderBy(x => x.FullName)
                                                     .Take(8)
                                                     .ToListAsync(cancellationToken))
                {
                    yield return new MediaInfoDto
                    {
                        Id = mediaInfo.Id,
                        Name = mediaInfo.Name,
                        FullName = mediaInfo.FullName,
                        PreviewImage = mediaInfo.PreviewImage,
                        CreationTime = mediaInfo.CreationTime.ToLocalTime().ToString("MM/dd/yyyy HH:mm:ss")
                    };
                }
            }
        }
    }
}
