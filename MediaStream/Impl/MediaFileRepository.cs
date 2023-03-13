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
                    : dbContext.MediaInfos.Where(x => x.FullSearchText.Contains(fileName!));
                
                foreach (var mediaInfo in await query.Where(x => !x.IsDeleted && MediaConstants.SupportedVideoExtensions.Contains(x.Extension))
                                                     .OrderBy(x => x.Theme)
                                                     .ThenBy(x => x.Name)
                                                     //ToDo lazy load or pagination
                                                     .Take(50)
                                                     .ToListAsync(cancellationToken))
                {
                    yield return new MediaInfoDto
                    {
                        Id = mediaInfo.Id,
                        Name = mediaInfo.Name,
                        Theme = mediaInfo.Theme,
                        IsLiked = mediaInfo.IsLiked,
                        FullName = mediaInfo.FullName,
                        PreviewImage = mediaInfo.PreviewImage,
                        CreationTime = mediaInfo.CreationTime.ToLocalTime().ToString("MM/dd/yyyy HH:mm:ss")
                    };
                }
            }
        }

        public async Task<MediaInfoDto?> MarkMediaAsLikesAsync(Guid id, CancellationToken cancellationToken) =>
            await UpdateIsLikedFlag(id, true, cancellationToken);

        public async Task<MediaInfoDto?> MarkMediaAsDislikedAsync(Guid id, CancellationToken cancellationToken) =>
            await UpdateIsLikedFlag(id, false, cancellationToken);

        private async Task<MediaInfoDto?> UpdateIsLikedFlag(Guid id, bool isLiked, CancellationToken cancellationToken)
        {
            using (var dbContext = _dbContextFactory.CreateContext())
            {
                var mediaInfo = await dbContext.MediaInfos.FindAsync(id);

                if (mediaInfo == null) throw new KeyNotFoundException($"Media info not found by {nameof(id)}: {id}");

                mediaInfo.IsLiked = isLiked;

                await dbContext.SaveChangesAsync(cancellationToken);

                return new MediaInfoDto
                {
                    Id = mediaInfo.Id,
                    Name = mediaInfo.Name,
                    Theme = mediaInfo.Theme,
                    IsLiked = mediaInfo.IsLiked,
                    FullName = mediaInfo.FullName,
                    PreviewImage = mediaInfo.PreviewImage,
                    CreationTime = mediaInfo.CreationTime.ToLocalTime().ToString("MM/dd/yyyy HH:mm:ss")
                };
            }
        }
    }
}
