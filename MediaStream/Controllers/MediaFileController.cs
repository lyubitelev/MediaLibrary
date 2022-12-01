﻿using MediaStream.Interfaces;
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
    }
}
