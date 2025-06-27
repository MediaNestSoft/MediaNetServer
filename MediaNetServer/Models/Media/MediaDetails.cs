namespace MediaNetServer.Models.Media;

public class GenreDto
{
    public int Id { get; set; }
    public required string Genre { get; set; }
}

public class PersonDto
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Department { get; set; }
    public string? ImagePath { get; set; }
}

public class MovieDetailDto
{
    public required string Title { get; set; }
    public required string Type { get; set; }           // "Movie"
    public string? PosterPath { get; set; }
    public string? BackdropPath { get; set; }
    public string? Country { get; set; }
    public int? Runtime { get; set; }           // 分钟
    public DateTime? ReleaseDate { get; set; }
    public string? Overview { get; set; }
    public IEnumerable<GenreDto>? Genre { get; set; }
    public IEnumerable<PersonDto>? Crew { get; set; }
    public double? VoteAverage { get; set; }
    public string? FileId { get; set; }
}

public class SeriesDetailDto
{
    public required string Title { get; set; }
    public required string Type { get; set; }           // "Series"
    public string? PosterPath { get; set; }
    public string? BackdropPath { get; set; }
    public string? LogoPath { get; set; }
    public DateTime? FirstAirDate { get; set; }
    public string? Country { get; set; }
    public IEnumerable<GenreDto>? Genre { get; set; }
    public double? VoteAverage { get; set; }
}


// 剧集与集

public class SeasonDto
{
    public int? SeasonNumber { get; set; }
    public string? Title { get; set; }
}

public class EpisodeDto
{
    public int? SeasonNumber { get; set; }
    public int? EpisodeNumber { get; set; }
    public string? Title { get; set; }
    public string? Overview { get; set; }
    public int? Runtime { get; set; }          // 分钟
    public DateTime? AirDate { get; set; }
    public double? UserRating { get; set; }
    public string? FileId { get; set; }
    public IEnumerable<PersonDto>? Cast { get; set; }
}

public class SeasonsResponseDto
{
    public IEnumerable<SeasonDto>? Seasons { get; set; }
}

public class EpisodesResponseDto
{
    public IEnumerable<EpisodeDto>? Episodes { get; set; }
}


// 媒体状态

public class MediaStatusDto
{
    public bool IsInContinueWatch { get; set; }
    public bool IsInFavorites { get; set; }
}
