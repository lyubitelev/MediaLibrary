using MediaStream.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace MediaStream.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaStreamController : ControllerBase
    {
        private readonly IMediaFileRepository _mediaFileRepository;
        private readonly ILogger<MediaStreamController> _logger;
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;
        private const string DefaultContentType = "video/webm";

        public MediaStreamController(IMediaFileRepository mediaFileRepository, ILogger<MediaStreamController> logger)
        {
            _mediaFileRepository = mediaFileRepository ?? throw new InvalidOperationException();
            _logger = logger ?? throw new InvalidOperationException();

            _fileExtensionContentTypeProvider = new FileExtensionContentTypeProvider();
        }

        [HttpGet("[action]/{fileName}")]
        public async Task<IActionResult> GetMediaStream(string fileName, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Get file stream by {nameof(fileName)}: {fileName}");

                var filePath = await _mediaFileRepository.GetFullPathByNameAsync(fileName, cancellationToken);

                var discoveredContentType = _fileExtensionContentTypeProvider.TryGetContentType(filePath, out var contentType)
                    ? contentType
                    : DefaultContentType;

                return File(new FileStream(filePath, FileMode.Open, FileAccess.Read), discoveredContentType, true);
            }
            catch (FileNotFoundException e)
            {
                _logger.LogError(e, "File search failed");
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Get media stream got error");
                throw;
            }
        }
    }
}
