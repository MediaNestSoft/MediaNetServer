namespace MediaNetServer.Models;

public class LocalSeriesDetail
{
    public int? MedaiId { get; set; }
    public string? Title { get; set; }
    public string Type { get; set; } = MediaType.Series;
    public string? Overview { get; set; }
    public List<string>? Genre { get; set; }
    public DateOnly? FirstAirDate { get; set; }
    public DateOnly? LastAirDate { get; set; }
    public int? NumberOfSeasons { get; set; }
    public int? NumberOfEpisodes { get; set; }
    public string? LogoPath { get; set; }
    public string? PosterPath { get; set; }
    public string? BackdropPath { get; set; }
    public float? Rating { get; set; }
    public string? Language { get; set; }
    
    public LocalSeriesDetail(int? tmDbId, string? title, string? overview, string? type,
        List<string>? genre, DateOnly? firstAirDate, DateOnly? lastAirDate,
        int? numberOfSeasons, int? numberOfEpisodes, string logoPath, string? posterPath,
        string? backdropPath, float? rating, string? language)
    {
        MedaiId = tmDbId;
        Title = title;
        Type = type;
        Overview = overview;
        Genre = genre;
        FirstAirDate = firstAirDate;
        LastAirDate = lastAirDate;
        NumberOfSeasons = numberOfSeasons;
        NumberOfEpisodes = numberOfEpisodes;
        LogoPath = logoPath;
        PosterPath = posterPath;
        BackdropPath = backdropPath;
        Rating = rating;
        Language = language;
    }
}