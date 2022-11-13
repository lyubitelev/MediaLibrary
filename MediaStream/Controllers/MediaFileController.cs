using System.Net;
using MediaStream.Interfaces;
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
            _mediaFileRepository = mediaFileRepository;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetAllVideoFileInfos")]
        public async Task<IActionResult> GetAllVideoFileInfos(CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _mediaFileRepository.GetAllVideoFileInfosAsync(cancellationToken));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Get all video file infos got error");

                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
