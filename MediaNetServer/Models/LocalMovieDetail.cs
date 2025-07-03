namespace MediaNetServer.Models;

public class LocalMovieDetail
{
    public int? MediaId { get; set; }
    public string? Title { get; set; }
    public string Type { get; set; } = MediaType.Movie;
    public string? Overview { get; set; }
    public List<string>? Genres { get; set; }
    public int? Runtime { get; set; }
    public DateOnly? ReleaseDate { get; set; }
    public string? LogoPath { get; set; }
    public string? PosterPath { get; set; }
    public string? BackdropPath { get; set; }
    public float? Rating { get; set; }
    public string? Language { get; set; }

    public LocalMovieDetail(int? tmDbId, string? title, string? type, string? overview,
        List<string>? genre, int? duration, DateOnly? releaseDate,
        string logoPath, string? posterPath, string? backdropPath, float? rating, string? language )
    {
        MediaId = tmDbId;
        Title = title;
        Type = type;
        Overview = overview;
        Genres = genre;
        Runtime = duration;
        ReleaseDate = releaseDate;
        LogoPath = logoPath;
        PosterPath = posterPath;
        BackdropPath = backdropPath;
        Rating = rating;
        Language = language;
    }
}