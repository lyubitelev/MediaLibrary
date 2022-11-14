using MediaStream.Constants;
using MediaStream.Interfaces;
using MediaStream.Models;
using Microsoft.Extensions.Options;

namespace MediaStream.Impl
{
    public class MediaFileRepository : IMediaFileRepository
    {
        private readonly DirectoryInfo _searchDirectory;
        
        public MediaFileRepository(IOptions<AppSettings> appSettings)
        {
            _searchDirectory = new DirectoryInfo(appSettings.Value.SearchDirectory ?? throw new InvalidOperationException());
        }

        public Task<string> GetFullPathByNameAsync(string fileName, CancellationToken cancellationToken)
        {
            var firstFoundFile = _searchDirectory.EnumerateFiles($"*{fileName}*.*", SearchOption.AllDirectories)
                                                 .FirstOrDefault();

            if (!MediaConstants.SupportedVideoExtensions.Contains(firstFoundFile?.Extension))
            {
                throw new FileNotFoundException($"Matching files not found, {nameof(fileName)}: {fileName}");
            }
            
            return Task.FromResult(firstFoundFile!.FullName);
        }

        //ToDo think about more asynchrony with IAsyncResult
        public async Task<IEnumerable<MediaInfoDto>> GetAllVideoFileInfosAsync(SearchMediaFilterDto mediaFilterDto, CancellationToken cancellationToken) =>
            await Task.Run(() =>
            {
                var searchFilePattern = string.IsNullOrEmpty(mediaFilterDto.FileName) ? "*.*" : $"*{mediaFilterDto.FileName}*.*";

                return _searchDirectory.EnumerateFiles(searchFilePattern, SearchOption.AllDirectories)
                                       .Where(x => MediaConstants.SupportedVideoExtensions.Contains(x.Extension))
                                       //ToDo think about pagination
                                       .Take(8)
                                       .Select(y => new MediaInfoDto
                                       {
                                           Name = Path.GetFileNameWithoutExtension(y.Name),
                                           FullName = y.Name,
                                           CreationTime = y.CreationTimeUtc.ToLocalTime()
                                                                           .ToString("MM/dd/yyyy HH:mm:ss"),
                                           //ToDo need added previewImage
                                           //PreviewImage = 
                                       })
                                       .ToList();
            }, cancellationToken);
    }
}
