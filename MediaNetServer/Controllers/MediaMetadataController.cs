using Microsoft.AspNetCore.Mvc;
using Org.OpenAPITools.Model;
using MediaNetServer.Data.media.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Org.OpenAPITools.Client;

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
    private readonly string _imagesRoot;
    private readonly PlaylistService _playlistService;

    public MediaMetadataController(
        MediaItemService mediaItemService,
        MovieDetailService movieDetailService,
        SeriesDetailService seriesDetailService,
        SeasonService seasonService,
        EpisodesService episodesService,
        MediaCastService mediaCastService,
        FilesService filesService,
        ImagesService imagesService,
        ILogger<MediaMetadataController> logger,
        IOptions<ImagesService.ImageSettings> imageSettings,
        PlaylistService playlistService)
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
        _imagesRoot = imageSettings.Value.CachePath;
        _playlistService = playlistService;
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

            if (mediaItem.Type == "Movie")
            {
                var movieDetail = await _movieDetailService.GetByMediaIdAsync(mediaId);
                var movieResponse = new MovieDetail(
                    new Option<int?>(movieDetail.MediaItem.TMDbId),
                    new Option<string?>(movieDetail.MediaItem.Title),
                    new Option<MovieDetail.TypeEnum?>(MovieDetail.TypeEnum.Movie),
                    new Option<string?>(movieDetail.Overview),
                    new Option<List<string>?>(movieDetail.MediaItem.Genre),
                    new Option<int?>(movieDetail.Duration),
                    new Option<DateOnly?>(DateOnly.FromDateTime(movieDetail.MediaItem.ReleaseDate)),
                    new Option<string?>(movieDetail.MediaItem.LogoPath),
                    new Option<string?>(movieDetail.MediaItem.PosterPath),
                    new Option<string?>(movieDetail.MediaItem.BackdropPath),
                    new Option<float?>((float)movieDetail.MediaItem.Rating),
                    new Option<string?>(movieDetail.MediaItem.Language)
                );
                var response = new GetMediaDetail200Response(movieResponse);
                return Ok(response);
            }
            else if (mediaItem.Type == "Series")
            {
                var seriesDetail = await _seriesDetailService.GetByIdAsync(mediaId);
                var seriesResponse = new SeriesDetail(
                    new Option<int?>(seriesDetail.MediaItem.TMDbId),
                    new Option<string?>(seriesDetail.MediaItem.Title),
                    new Option<SeriesDetail.TypeEnum?>(SeriesDetail.TypeEnum.Series),
                    new Option<string?>(seriesDetail.overview),
                    new Option<List<string>?>(seriesDetail.MediaItem.Genre),
                    new Option<DateOnly?>(DateOnly.FromDateTime(seriesDetail.firstAirDate)),
                    new Option<DateOnly?>(DateOnly.FromDateTime(seriesDetail.lastAirDate)),
                    new Option<int?>(seriesDetail.numberOfSeasons),
                    new Option<int?>(seriesDetail.numberOfEpisodes),
                    new Option<string?>(seriesDetail.MediaItem.LogoPath),
                    new Option<string?>(seriesDetail.MediaItem.PosterPath),
                    new Option<string?>(seriesDetail.MediaItem.BackdropPath),
                    new Option<float?>((float)seriesDetail.MediaItem.Rating),
                    new Option<string?>(seriesDetail.MediaItem.Language)
                );

                var response = new GetMediaDetail200Response(seriesResponse);
                return Ok(response);
            }

            return BadRequest(new Error { Message = "Unknown media type" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media detail for {SeasonId}", mediaId);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    [HttpGet("series/seasons/episodes/credits")]
    public async Task<IActionResult> GetEpisodeCredits([FromQuery] int seriesId, [FromQuery] int seasonNumber,
        [FromQuery] int episodeNumber)
    {
        try
        {
            var episodeId = await _episodesService.GetEpisode(seriesId, seasonNumber, episodeNumber);
            var credits = await _mediaCastService.GetEpisodeCastAsync(episodeId.tmdbId);
            var creditList = credits.Select(c => new Credit
            {
                Name = c.Name,
                Department = c.Department,
                PhotoPath = c.PersonUrl
            }).ToList();

            var response = new CreditsResponse
            {
                Credits = creditList
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error getting episode credits for series {SeriesId}, season {SeasonNumber}, episode {EpisodeNumber}",
                seriesId, seasonNumber, episodeNumber);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    [HttpGet("episodes")]
    public async Task<IActionResult> GetEpisodesList([FromQuery] int seriesId, [FromQuery] int seasonNumber)
    {
        try
        {
            var episodes = await _episodesService.GetEpisodesBySeriesAndSeasonAsync(seriesId, seasonNumber);
            var episodeList = episodes
                .Select(e => new Episode(
                    episodeNumber: new Option<int?>(e.episodeNumber),
                    title: new Option<string?>(e.episodeName),
                    overview: new Option<string?>(e.overview),
                    stillPath: new Option<string?>(e.stillPath),
                    airDate: new Option<DateOnly?>(DateOnly.FromDateTime(e.airDate)),
                    duration: new Option<int?>(e.duration),
                    rating: new Option<float?>((float)e.rating)
                ))
                .ToList();

            var response = new EpisodesResponse
            {
                Episodes = episodeList
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting episodes for series {SeriesId}, season {SeasonNumber}", seriesId,
                seasonNumber);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    [HttpGet("seasons")]
    public async Task<IActionResult> GetSeasonsList([FromQuery] int seriesId)
    {
        try
        {
            var seasons = await _seasonService.GetSeasonsByMediaIdAsync(seriesId);
            var seasonList = seasons
                .Select(s => new Season(
                    seasonNumber: new Option<int?>(s.SeasonNumber),
                    name: new Option<string?>(s.SeasonName),
                    overview: new Option<string?>(s.overview),
                    posterPath: new Option<string?>(s.posterPath),
                    airDate: new Option<DateOnly?>(DateOnly.FromDateTime(s.AirDate)),
                    episodeCount: new Option<int?>(s.episodeCount),
                    rating: new Option<float?>(s.rating)
                ))
                .ToList();

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
            var cleanedPath = path.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var fileOnDisk = Path.GetFullPath(Path.Combine(_imagesRoot, cleanedPath));
            // 防止越界
            if (!fileOnDisk.StartsWith(_imagesRoot, StringComparison.OrdinalIgnoreCase))
                return BadRequest(new Error { Message = "Invalid image path" });

            if (!System.IO.File.Exists(fileOnDisk))
            {
                Console.WriteLine($"Image file not found: {fileOnDisk}");
                return NotFound(new Error { Message = "Image not found" });
            }

            var data = await System.IO.File.ReadAllBytesAsync(fileOnDisk);
            var contentType = GetContentType(fileOnDisk);
            return File(data, contentType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting image {Path}", path);
            Console.WriteLine(ex.ToString());
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    private string GetContentType(string filePath)
    {
        var ext = Path.GetExtension(filePath).ToLowerInvariant();
        return ext switch
        {
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetMediaStatus([FromQuery] string userId, [FromQuery] int mediaId)
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
            bool fav = await _playlistService.IsFavoriteAsync(userId, mediaId);
            bool cnt = await _playlistService.IsInContinueWatchAsync(userId, mediaId);

            var response = new MediaStatusResponse(
                isInFavorites: new Option<bool?>(fav),
                isInContinueWatch: new Option<bool?>(cnt)
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media status for user {UserId}, media {SeasonId}", userId, mediaId);
            return StatusCode(500, new Error { Message = "Internal server error" });
        }
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetMediaList([FromQuery] int limit = 20, [FromQuery] int offset = 0,
        [FromQuery] string? type = null)
    {
        try
        {
            var allMedia = await _mediaItemService.GetMediaListAsync(type);
            int totalCount = allMedia.Count;
            int skip = offset * limit;
            if (skip < 0) skip = 0;

            var pageMedia = allMedia
                .Skip(skip)
                .Take(limit)
                .ToList();

            var mediaItems = new List<MediaItem>();
            foreach (var m in pageMedia)
            {
                bool isMovie = string.Equals(m.Type, "movie", StringComparison.OrdinalIgnoreCase);
                var typeEnum = isMovie
                    ? MediaItem.TypeEnum.Movie
                    : MediaItem.TypeEnum.Series;

                string? additional = isMovie
                    ? m.ReleaseDate.Year.ToString()
                    : (await _episodesService.GetByEpisodeImdbIdAsync(m.TMDbId))?.seasonNumber.ToString();

                mediaItems.Add(new MediaItem(
                    mediaId: new Option<int?>(m.TMDbId),
                    title: new Option<string?>(m.Title),
                    type: new Option<MediaItem.TypeEnum?>(typeEnum),
                    posterPath: new Option<string?>(m.PosterPath),
                    additional: !string.IsNullOrEmpty(additional)
                        ? new Option<string?>(additional)
                        : default
                ));
            }

            var response = new MediaListResponse
            {
                Items = mediaItems,
                TotalCount = totalCount
            };
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting media list}");
            return StatusCode(500, new Error { Message = "Internal server error" });
        }

    }
}
