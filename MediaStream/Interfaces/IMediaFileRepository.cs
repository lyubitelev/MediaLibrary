using MediaStream.Models;

namespace MediaStream.Interfaces
{
    public interface IMediaFileRepository
    {
        Task<string> GetFullPathByNameAsync(string fileName, CancellationToken cancellationToken);

        Task<IEnumerable<MediaInfoDto>> GetAllVideoFileInfosAsync(SearchMediaFilterDto mediaFilterDto,
                                                                  CancellationToken cancellationToken);
    }
}
