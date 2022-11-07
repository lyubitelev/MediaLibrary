using System.Net;
using MediaStream.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace MediaStream.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaStreamController : ControllerBase, IDisposable
    {
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;
        private readonly IMediaFileRepository _mediaFileRepository;
        private readonly ILogger<MediaStreamController> _logger;
        private const string DefaultContentType = "video/webm";

        private FileStream? _localFileStream;

        public MediaStreamController(IMediaFileRepository mediaFileRepository, ILogger<MediaStreamController> logger)
        {
            _mediaFileRepository = mediaFileRepository;
            _fileExtensionContentTypeProvider = new FileExtensionContentTypeProvider();
            _logger = logger;
        }

        [HttpGet]
        [Route("GetMediaStream/{fileName}")]
        public async Task<IActionResult> GetMediaStream(string fileName, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Get file stream by {nameof(fileName)}: {fileName}");

                var filePath = await _mediaFileRepository.GetFullPathByNameAsync(fileName, cancellationToken);

                _localFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                var discoveredContentType = _fileExtensionContentTypeProvider.TryGetContentType(filePath, out var contentType)
                    ? contentType
                    : DefaultContentType;

                return File(_localFileStream, discoveredContentType, true);
            }
            catch (FileNotFoundException e)
            {
                _logger.LogError(e, "File search failed");

                return StatusCode((int)HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Get media stream got error");

                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        public void Dispose()
        {
            _localFileStream?.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
