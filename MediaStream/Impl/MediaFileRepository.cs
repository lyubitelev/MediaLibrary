using System.Net;
using MediaStream.Interfaces;
using MediaStream.Models;
using Microsoft.Extensions.Options;

namespace MediaStream.Impl
{
    public class MediaFileRepository : IMediaFileRepository
    {
        private readonly DirectoryInfo _searchDirectory;
        private readonly byte[] _testPreviewImage;
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

            //ToDo do not forget to delete and change to something more reasonable
            using var webClient = new WebClient();
            _testPreviewImage = webClient.DownloadData("https://material.angular.io/assets/img/examples/shiba2.jpg");
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

        //ToDo think about more asynchrony with IAsyncResult
        public async Task<IEnumerable<MediaInfoDto>> GetAllVideoFileInfosAsync(CancellationToken cancellationToken) =>
            await Task.Run(() =>
            {
                return _searchDirectory.GetFiles("*.*", SearchOption.AllDirectories)
                                       .Where(x => _fileExtensions.Contains(x.Extension))
                                       //ToDo add pagination
                                       .Take(8)
                                       .Select(y => new MediaInfoDto
                                       {
                                           Name = Path.GetFileNameWithoutExtension(y.Name),
                                           FullName = y.Name,
                                           CreationTime = y.CreationTimeUtc
                                               .ToLocalTime()
                                               .ToString("MM/dd/yyyy HH:mm:ss"),
                                           PreviewImage = _testPreviewImage
                                       }).ToList();
            }, cancellationToken);
    }
}
