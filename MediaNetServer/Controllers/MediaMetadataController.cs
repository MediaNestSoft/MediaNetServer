using Microsoft.AspNetCore.Mvc;
using Org.OpenAPITools.Model;
using MediaNetServer.Data.media.Services;
using Microsoft.Extensions.Logging;

namespace MediaNetServer.Controllers;

[ApiController]
[Route("media")]
public class MediaMetadataController : ControllerBase
{
    private readonly MediaItemService _mediaItemService;
    private readonly MovieDetailService _movieDetailService;
    private readonly SeriesDetailService _seriesDetailService;
    private readonly SeasonService _seasonService;
    private readonly EpisodesService _episodesService;
    private readonly MediaCastService _mediaCastService;
    private readonly FilesService _filesService;
    private readonly ImagesService _imagesService;
    private readonly ILogger<MediaMetadataController> _logger;

    public MediaMetadataController(
        MediaItemService mediaItemService,
        MovieDetailService movieDetailService,
        SeriesDetailService seriesDetailService,
        SeasonService seasonService,
        EpisodesService episodesService,
        MediaCastService mediaCastService,
        FilesService filesService,
        ImagesService imagesService,
        ILogger<MediaMetadataController> logger)
    {
        _mediaItemService = mediaItemService;
        _movieDetailService = movieDetailService;
        _seriesDetailService = seriesDetailService;
        _seasonService = seasonService;
        _episodesService = episodesService;
        _mediaCastService = mediaCastService;
        _filesService = filesService;
        _imagesService = imagesService;
        _logger = logger;
    }

    [HttpGet("detail")]
    public async Task<IActionResult> GetMediaDetail([FromQuery] int mediaId)
    {
        try
        {
            var mediaItem = await _mediaItemService.GetMediaItemByIdAsync(mediaId);
            if (mediaItem == null)
            {
                return NotFound(new Error { Message = "Media not found" });
            }

            if (mediaItem.Type == "movie")
            {
                var movieDetail = await _movieDetailService.GetByMediaIdAsync(mediaId);
                var movieResponse = new MovieDetail(
                    movieDetail.MediaItem.TMDbId,
                    movieDetail.MediaItem.Title,
                    (MovieDetail.TypeEnum?)1,
                    movieDetail.Overview,
                    movieDetail.MediaItem.Genre,
                    movieDetail.Duration,
                    DateOnly.FromDateTime(movieDetail.MediaItem.ReleaseDate),
                    movieDetail.MediaItem.LogoPath,
                    movieDetail.MediaItem.PosterPath,
                    movieDetail.MediaItem.BackdropPath,
                    (float)movieDetail.MediaItem.Rating,
                    movieDetail.MediaItem.Language
                );
                var response = new GetMediaDetail200Response(movieResponse);
                return Ok(response);
            }
            else if (mediaItem.Type == "series")
            {
                var seriesDetail = await _seriesDetailService.GetByIdAsync(mediaId);
                var seriesResponse = new SeriesDetail(
                    seriesDetail.MediaItem.TMDbId,
                    seriesDetail.MediaItem.Title,
                    (SeriesDetail.TypeEnum?)2,
                    seriesDetail.overview,
                    seriesDetail.MediaItem.Genre,
                    DateOnly.FromDateTime(seriesDetail.firstAirDate),
                    DateOnly.FromDateTime(seriesDetail.lastAirDate),
                    seriesDetail.numberOfSeasons,
                    seriesDetail.numberOfEpisodes,
                    seriesDetail.MediaItem.LogoPath,
                    seriesDetail.MediaItem.PosterPath,
                    seriesDetail.MediaItem.BackdropPath,
                    (float)seriesDetail.MediaItem.Rating,
                    seriesDetail.MediaItem.Language
                );
                
                var response = new GetMediaDetail200Response(seriesResponse);
                return Ok(response);
            }

            return BadRequest(new Error { Message = "Unknown media type" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media detail for {MediaId}", mediaId);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    [HttpGet("series/seasons/episodes/credits")]
    public async Task<IActionResult> GetEpisodeCredits([FromQuery]int seriesId, [FromQuery]int seasonNumber, [FromQuery]int episodeNumber)
    {
        try
        {
            var episode = await _episodesService.GetEpisodeBySeriesSeasonAndNumberAsync(seriesId, seasonNumber, episodeNumber);
            if (episode == null)
            {
                return NotFound(new Error { Message = "Episode not found" });
            }

            var credits = await _mediaCastService.GetByMediaIdAsync(episode.mediaId);
            var creditList = credits.Select(c => new Credit
            {
                Id = c.CastId,
                Name = c.Name,
                Character = c.Character,
                Job = c.Job,
                ProfilePath = c.ProfilePath
            }).ToList();

            var response = new CreditsResponse
            {
                Credits = creditList
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting episode credits for series {SeriesId}, season {SeasonNumber}, episode {EpisodeNumber}", 
                seriesId, seasonNumber, episodeNumber);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    [HttpGet("media/episodes")]
    public async Task<IActionResult> GetEpisodesList([FromQuery]int seriesId, [FromQuery]int seasonNumber)
    {
        try
        {
            var episodes = await _episodesService.GetEpisodesBySeriesAndSeasonAsync(seriesId, seasonNumber);
            var episodeList = episodes.Select(e => new Episode
            {
                Id = e.EpisodeId,
                EpisodeNumber = e.EpisodeNumber,
                Name = e.Name,
                Overview = e.Overview,
                StillPath = e.StillPath,
                AirDate = e.AirDate,
                Runtime = e.Runtime
            }).ToList();

            var response = new EpisodesResponse
            {
                Episodes = episodeList
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting episodes for series {SeriesId}, season {SeasonNumber}", seriesId, seasonNumber);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    [HttpGet("seasons/{seriesId}")]
    public async Task<IActionResult> GetSeasonsList(int seriesId)
    {
        try
        {
            var seasons = await _seasonService.GetSeasonsByMediaIdAsync(seriesId);
            var seasonList = seasons.Select(s => new Season
            {
                Id = s.SeasonId,
                SeasonNumber = s.SeasonNumber,
                Name = s.Name,
                Overview = s.Overview,
                PosterPath = s.PosterPath,
                AirDate = s.AirDate,
                EpisodeCount = s.EpisodeCount
            }).ToList();

            var response = new SeasonsResponse
            {
                Seasons = seasonList
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting seasons for series {SeriesId}", seriesId);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    [HttpGet("image/{*path}")]
    public async Task<IActionResult> GetMediaImage(string path)
    {
        try
        {
            var image = await _imagesService.GetImageByPathAsync(path);
            if (image?.ImageData == null)
            {
                return NotFound(new Error { Message = "Image not found" });
            }

            // 返回图片的二进制数据
            return File(image.ImageData, "image/jpeg"); // 或根据实际格式调整
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting image {Path}", path);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    [HttpGet("status/{userId}/{mediaId}")]
    public async Task<IActionResult> GetMediaStatus(string userId, int mediaId)
    {
        try
        {
            var mediaItem = await _mediaItemService.GetMediaItemByIdAsync(mediaId);
            if (mediaItem == null)
            {
                return NotFound(new Error { Message = "Media not found" });
            }

            // 这里需要实现获取用户对媒体的状态逻辑
            // 暂时返回基本信息
            var response = new MediaStatusResponse
            {
                MediaId = mediaId,
                IsWatched = false, // 需要从WatchProgress获取
                IsFavorite = false, // 需要从用户收藏获取
                WatchProgress = 0 // 需要从WatchProgress获取
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media status for user {UserId}, media {MediaId}", userId, mediaId);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }
}
