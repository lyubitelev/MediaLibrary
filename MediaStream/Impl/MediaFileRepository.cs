using MediaStream.Interfaces;
using Microsoft.Extensions.Options;

namespace MediaStream.Impl
{
    public class MediaFileRepository : IMediaFileRepository
    {
        private readonly DirectoryInfo _searchDirectory;
        private readonly IEnumerable<string> _fileExtensions = new List<string>
        {
            ".3g2",
            ".3gp",
            ".avi",
            ".flv",
            ".h26",
            ".m4v",
            ".mkv",
            ".mov",
            ".mp4",
            ".mpg",
            ".mpeg",
            ".rm",
            ".swf",
            ".vob",
            ".wmv"
        };

        public MediaFileRepository(IOptions<AppSettings> appSettings)
        {
            _searchDirectory = new DirectoryInfo(appSettings.Value.SearchDirectory!);
        }

        public Task<string> GetFullPathByNameAsync(string fileName, CancellationToken cancellationToken)
        {
            var firstFoundFile = _searchDirectory.GetFiles($"*{fileName}*.*", SearchOption.AllDirectories)
                                                 .FirstOrDefault();

            if (firstFoundFile == null || !_fileExtensions.Contains(firstFoundFile.Extension))
            {
                throw new FileNotFoundException($"Matching files not found, {nameof(fileName)}: {fileName}");
            }
            
            return Task.FromResult(firstFoundFile.FullName);
        }

        public Task<IEnumerable<string>> GetAllVideoFilesNameAsync(CancellationToken cancellationToken) =>
            Task.FromResult(_searchDirectory.GetFiles("*.*", SearchOption.AllDirectories)
                                            .Where(x => _fileExtensions.Contains(x.Extension))
                                            .Select(y => Path.GetFileNameWithoutExtension(y.Name)));
    }
}
