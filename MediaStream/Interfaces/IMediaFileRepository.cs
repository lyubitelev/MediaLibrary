using MediaStream.Models;

namespace MediaStream.Interfaces
{
    public interface IMediaFileRepository
    {
        Task<string> GetFullPathByNameAsync(string fileName, CancellationToken cancellationToken);

        IAsyncEnumerable<MediaInfoDto> GetAllVideoFileInfosAsync(SearchMediaFilterDto mediaFilterDto,
                                                                 CancellationToken cancellationToken);
        Task<MediaInfoDto?> MarkMediaAsLikesAsync(Guid id, CancellationToken cancellationToken);
        Task<MediaInfoDto?> MarkMediaAsDislikedAsync(Guid id, CancellationToken cancellationToken);
    }
}
