using Microsoft.AspNetCore.Mvc;
using Org.OpenAPITools.Model;
using MediaNetServer.Data.media.Services;
using Microsoft.Extensions.Logging;

namespace MediaNetServer.Controllers;

[ApiController]
[Route("stream")]
public class StreamingController : ControllerBase
{
    private readonly FilesService _filesService;
    private readonly MediaItemService _mediaItemService;
    private readonly EpisodesService _episodesService;
    private readonly ILogger<StreamingController> _logger;

    public StreamingController(FilesService filesService, MediaItemService mediaItemService,
        EpisodesService episodesService, ILogger<StreamingController> logger)
    {
        _filesService = filesService;
        _mediaItemService = mediaItemService;
        _episodesService = episodesService;
        _logger = logger;
    }

    [HttpGet("file/{fileId}")]
    public async Task<IActionResult> StreamMedia(string fileId)
    {
        try
        {
            var file = await _filesService.GetByIdAsync(fileId);
            if (file == null)
            {
                return NotFound(new Error { Message = "File not found" });
            }

            // 检查文件是否存在
            if (!System.IO.File.Exists(file.FilePath))
            {
                return NotFound(new Error { Message = "Physical file not found" });
            }

            // 返回文件流，支持Range请求
            var fileStream = new FileStream(file.FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var contentType = GetContentType(file.FilePath);
            
            return File(fileStream, contentType, enableRangeProcessing: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error streaming file {FileId}", fileId);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    [HttpGet("movie/{tmdbId}/files")]
    public async Task<IActionResult> GetMovieFiles(int mediaId)
    {
        try
        {
            var mediaItem = await _mediaItemService.GetMediaItemByIdAsync(mediaId);
            if (mediaItem?.Type != "movie")
            {
                return NotFound(new Error { Message = "Movie not found" });
            }

            var files = await _filesService.GetFilesByMediaIdAsync(mediaId);
            var mediaFiles = files.Select(f => new MediaFile
            {
                Id = f.FileId,
                Path = f.FilePath,
                Size = f.FileSize,
                Quality = f.Quality,
                Resolution = f.Resolution,
                Codec = f.Codec,
                Container = f.Container
            }).ToList();

            var response = new MediaFilesResponse
            {
                Files = mediaFiles
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting movie files for {MediaId}", mediaId);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    [HttpGet("series/{seriesId}/seasons/{seasonNumber}/episodes/{episodeNumber}/files")]
    public async Task<IActionResult> GetEpisodeFiles(int seriesId, int seasonNumber, int episodeNumber)
    {
        try
        {
            var episode = await _episodesService.GetEpisodeBySeriesSeasonAndNumberAsync(seriesId, seasonNumber, episodeNumber);
            if (episode == null)
            {
                return NotFound(new Error { Message = "Episode not found" });
            }

            var files = await _filesService.GetFilesByEpisodeIdAsync(episode.EpisodeId);
            var mediaFiles = files.Select(f => new MediaFile
            {
                Id = f.FileId,
                Path = f.FilePath,
                Size = f.FileSize,
                Quality = f.Quality,
                Resolution = f.Resolution,
                Codec = f.Codec,
                Container = f.Container
            }).ToList();

            var response = new MediaFilesResponse
            {
                Files = mediaFiles
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting episode files for series {SeriesId}, season {SeasonNumber}, episode {EpisodeNumber}", 
                seriesId, seasonNumber, episodeNumber);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    private string GetContentType(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension switch
        {
            ".mp4" => "video/mp4",
            ".mkv" => "video/x-matroska",
            ".avi" => "video/x-msvideo",
            ".mov" => "video/quicktime",
            ".wmv" => "video/x-ms-wmv",
            ".webm" => "video/webm",
            _ => "application/octet-stream"
        };
    }
}
