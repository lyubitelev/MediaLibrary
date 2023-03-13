using MediaStream.Interfaces;
using MediaStream.Models;
using Microsoft.AspNetCore.Mvc;

namespace MediaStream.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaFileController : ControllerBase
    {
        private readonly IMediaFileRepository _mediaFileRepository;
        private readonly ILogger<MediaStreamController> _logger;

        public MediaFileController(IMediaFileRepository mediaFileRepository, ILogger<MediaStreamController> logger)
        {
            _mediaFileRepository = mediaFileRepository ?? throw new InvalidOperationException();
            _logger = logger ?? throw new InvalidOperationException();
        }

        [HttpGet("[action]/{fileName?}")]
        public IAsyncEnumerable<MediaInfoDto> GetAllVideoFileInfos(CancellationToken cancellationToken, string? fileName = null)
        {
            try
            {
                _logger.LogInformation($"User agent connected: {HttpContext.Request.Headers["User-Agent"]}");
                _logger.LogInformation($"Get all video file infos start by filter: {fileName}");

                return _mediaFileRepository.GetAllVideoFileInfosAsync(new SearchMediaFilterDto { FileName = fileName! },
                                                                      cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Get all video file infos got error");
                throw;
            }
        }

        [HttpPost("[action]/{id}")]
        public async Task<IActionResult> MarkMediaAsLikes(string id, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Mark media as likes by id: {id}");

                var guid = Guid.Parse(id);
                var mediaInfoDto = await _mediaFileRepository.MarkMediaAsLikesAsync(guid, cancellationToken);

                return Ok(mediaInfoDto);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Mark media as likes got error");
                throw;
            }
        }

        [HttpPost("[action]/{id}")]
        public async Task<IActionResult> MarkMediaAsDisliked(string id, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Mark media as disliked by id: {id}");

                var guid = Guid.Parse(id);
                var mediaInfoDto = await _mediaFileRepository.MarkMediaAsDislikedAsync(guid, cancellationToken);

                return Ok(mediaInfoDto);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Mark media as likes got error");
                throw;
            }
        }
    }
}
