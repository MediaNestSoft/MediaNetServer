using Microsoft.AspNetCore.Mvc;
using Org.OpenAPITools.Model;
using MediaNetServer.Data.media.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace MediaNetServer.Controllers;


[ApiController]
[Route("playback")]
public class StreamingController : ControllerBase
{
    private readonly FilesService _filesService;
    private readonly MediaItemService _mediaItemService;
    private readonly EpisodesService _episodesService;
    private readonly ILogger<StreamingController> _logger;
    private readonly MovieDetailService _movieDetailService;

    public StreamingController(FilesService filesService, MediaItemService mediaItemService,
        EpisodesService episodesService, ILogger<StreamingController> logger, MovieDetailService movieDetailService)
    {
        _filesService = filesService;
        _mediaItemService = mediaItemService;
        _episodesService = episodesService;
        _logger = logger;
        _movieDetailService = movieDetailService;
    }

    [HttpGet("stream")]
    public async Task<IActionResult> StreamMedia([FromQuery]string fileId)
    {
        try
        {
            var file = await _filesService.GetFileByFid(fileId);
            if (file == null)
            {
                return NotFound(new Error { Message = "File not found" });
            }

            // 检查文件是否存在
            if (!System.IO.File.Exists(file.filePath))
            {
                return NotFound(new Error { Message = "Physical file not found" });
            }

            // 返回文件流，支持Range请求
            var fileStream = new FileStream(file.filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var contentType = GetContentType(file.filePath);
            
            return File(fileStream, contentType, enableRangeProcessing: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error streaming file {FileId}", fileId);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    [HttpGet("movie/files")]
    public async Task<IActionResult> GetMovieFiles([FromQuery] int mediaId)
    {
        try
        {
            var mediaItem = await _movieDetailService.GetMovieDurationAsync(mediaId);
            var files = await _filesService.GetByIdAsync(mediaId);
            var mediaFiles = files.Select(f => new MediaFile
            {
                FileId = f.fid.ToString(),
                Size = 100,
                Quality = "1080P",
                Container = "mkv",
                Language = "zh-CN",
                Duration = (int)(mediaItem * 60)
            }).ToList();

            var response = new MediaFilesResponse
            {
                Files = mediaFiles
            };

            return Ok(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
/*
    [HttpGet("movie/{SeasonId}/files")]
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
            _logger.LogError(ex, "Error getting movie files for {SeasonId}", mediaId);
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
    }*/

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